using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models.Users;
using AnalyseProgra.Models;
using AnalyseProgra.Views;
using AnalyseProgra.Interactions;
using AnalyseProgra.DataAccess.Dao;

class Program
{
    //static Player _player = new Player("Justin", "Just123", new UserDao());
    static Player _player = new UserDao().GetById(1, true) as Player;
    static Colony _colony = _player.Colony;
    static object _verrouBatiments = new object();
    static bool _jeuEnCours = true;

    static async Task Main(string[] args)
    {
        _player.AddActions();   // Ne peut pas être dans le constructeur car Colony n'est pas encore initialisée
        
        /*_colony.Buildings.AddBuilding(BuildingType.IronMine, 1);
        _colony.Buildings.AddBuilding(BuildingType.IronMine, 1);
        _colony.Buildings.AddBuilding(BuildingType.IronMine, 1);
        _colony.Buildings.AddBuilding(BuildingType.IronMine, 2);
        _colony.Buildings.AddBuilding(BuildingType.IronMine, 2);*/

        PlayerUI ui = new PlayerUI(_player);
        
        var tacheMoteur = Task.Run(BoucleDeJeu);

        while (_jeuEnCours)
        {
            Interaction chosenAction = await ui.ShowDashboard();

            if (_jeuEnCours)
            {
                ui.ClearScreen();
                chosenAction.Execute(ui);
            }
        }

        await tacheMoteur;
    }


    static async Task BoucleDeJeu()
    {
        while (_jeuEnCours)
        {
            _colony.Population.UpdateMaxPopulation(_colony.Buildings);
            _colony.Resources.UpdateMaxStorage(_colony.Buildings);

            if (_colony.Buildings != null)
            {
                foreach (var stack in _colony.Buildings.BuildingStacks)
                {
                    switch (stack.BuildingType)
                    {
                        case BuildingType.Farm:
                            int productionNourriture = (stack.Level * 5) * stack.Amount;
                            _colony.Resources.Ajouter(ResourceType.Patate, productionNourriture);
                            break;

                        case BuildingType.IronMine:
                            int productionFer = (stack.Level * 2) * stack.Amount;
                            _colony.Resources.Ajouter(ResourceType.Fer, productionFer);
                            break;
                    }
                }
            }

            int nb = _colony.Population.GetStock();
            if (nb > 0)
            {
                if (_colony.Resources.HasEnough(ResourceType.Patate, nb)) _colony.Resources.Retirer(ResourceType.Patate, nb);
                else _colony.Population.Retirer(1);
            }
            await Task.Delay(1000);
        }
    }
}