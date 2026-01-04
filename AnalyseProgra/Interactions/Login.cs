using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models.Users;
using AnalyseProgra.UserInterfaces;

namespace AnalyseProgra.Interactions
{
    public class Login : Interaction
    {
        private readonly IUserDao _userDao;
        public User? SelectedUser { get; private set; }

        public Login(IUserDao userDao) : base("Login", "Créer ou sélectionner un utilisateur")
        {
            _userDao = userDao;
        }

        public override void Execute(IUserInterface ui)
        {
            var mode = ui.Select("Connexion", new[]
            {
                "Se connecter (sélectionner)",
                "Créer un utilisateur"
            });

            if (mode.StartsWith("Se connecter"))
            {
                var users = _userDao.GetAll().ToList();
                if (users.Count == 0)
                {
                    ui.WriteError("Aucun utilisateur en base. Création obligatoire.");
                    SelectedUser = CreateFlow(ui);
                    return;
                }

                var selected = ui.Select("Choisir un utilisateur", users,
                    u => $"{u.Id} · {u.Username} · {u.Role} · Actif={u.IsActive}");

                var loaded = _userDao.GetById(selected.Id, includeDetails: true);

                if (loaded == null)
                {
                    ui.WriteError("Erreur : impossible de charger l'utilisateur sélectionné.");
                    return;
                }

                for (int i = 0; i < 3; i++)
                {
                    string password_user = ui.AskPassword("Password");

                    if (loaded.PasswordHash == password_user)
                    {
                        SelectedUser = loaded;
                        return;
                    }

                    ui.WriteError("Mot de passe incorrect.");
                }

                SelectedUser = null;
                return;

            }


            SelectedUser = CreateFlow(ui);
        }

        private User CreateFlow(IUserInterface ui)
        {
            string username = ui.Ask<string>("Username").Trim();
            string password = ui.AskPassword("Password");

            var existing = _userDao.GetByUsername(username, includeDetails: true);
            if (existing != null)
            {
                ui.WriteError("Ce username existe déjà.");
                return existing;
            }

            var player = new Player(username, password, _userDao)
            {
                Role = UserRole.Player,
                IsActive = true
            };

            var created = _userDao.Create(player);
            return _userDao.GetById(created.Id, includeDetails: true)!;
        }



    }
}
