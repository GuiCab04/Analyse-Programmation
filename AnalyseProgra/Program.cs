using AnalyseProgra.DataAccess.Dao;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.Interactions;
using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models.Users;
using AnalyseProgra.Views;

class Program
{
    static bool _jeuEnCours = true;

    static async Task Main(string[] args)
    {
        IUserDao userDao = new UserDao();

        SeedUsersIfEmpty(userDao);

        var uiStart = new StartUI();
        var login = new Login(userDao);
        login.Execute(uiStart);

        var user = login.SelectedUser!;
        user.AddActions();

        if (user.Role == UserRole.Admin)
        {
            var adminUi = new RolePanelUI(user);
            while (true)
            {
                var action = adminUi.ShowMenu();
                adminUi.ClearScreen();
                action.Execute(adminUi);
            }
        }

        if (user.Role == UserRole.Moderator)
        {
            var choice = uiStart.Select("Mode", new[] { "Jouer", "Panneau modération" });

            if (choice == "Panneau modération")
            {
                var modUi = new RolePanelUI(user);
                while (true)
                {
                    var action = modUi.ShowMenu();
                    modUi.ClearScreen();
                    action.Execute(modUi);
                }
            }
        }

        var player = user as Player;

        if (player == null)
        {
            uiStart.WriteError("Ce compte n'est pas un joueur (Admin ne peut pas jouer).");
            return;
        }

        var ui = new PlayerUI(player);

        var tacheMoteur = Task.Run(() => BoucleDeJeu(player));

        while (_jeuEnCours)
        {
            var chosenAction = await ui.ShowDashboard();
            ui.ClearScreen();
            chosenAction.Execute(ui);
        }

        await tacheMoteur;
    }

    private class StartUI : SpectreInterface { }

    private static void SeedUsersIfEmpty(IUserDao userDao)
    {
        var users = userDao.GetAll().ToList();
        if (users.Count > 0) return;

        userDao.Create(new User("admin", "admin", userDao)
        {
            Role = UserRole.Admin,
            IsActive = true
        });

        userDao.Create(new User("moderator", "moderator", userDao)
        {
            Role = UserRole.Moderator,
            IsActive = true
        });

        userDao.Create(new User("player", "player", userDao)
        {
            Role = UserRole.Player,
            IsActive = true
        });
    }

    static async Task BoucleDeJeu(Player player)
    {
        var colony = player.Colony;

        while (_jeuEnCours)
        {
            if (!player.IsActive)
            {
                await Task.Delay(1000);
                continue;
            }

            colony.Population.UpdateMaxPopulation(colony.Buildings);
            colony.Resources.UpdateMaxStorage(colony.Buildings);

            foreach (var stack in colony.Buildings.BuildingStacks)
            {
                if (stack.BuildingType == BuildingType.Farm)
                    colony.Resources.Ajouter(ResourceType.Patate, (stack.Level * 5) * stack.Amount);

                if (stack.BuildingType == BuildingType.IronMine)
                    colony.Resources.Ajouter(ResourceType.Fer, (stack.Level * 2) * stack.Amount);
            }

            int nb = colony.Population.GetStock();
            if (nb > 0)
            {
                if (colony.Resources.HasEnough(ResourceType.Patate, nb)) colony.Resources.Retirer(ResourceType.Patate, nb);
                else colony.Population.Retirer(1);
            }

            await Task.Delay(1000);
        }
    }
}
