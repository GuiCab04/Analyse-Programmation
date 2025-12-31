using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models;
using AnalyseProgra.Views;
using AnalyseProgra.DataAccess.Dao;
using AnalyseProgra.Models.Buildings;

class Program
{
    static Colony _colony = new Colony();
    static object _verrouBatiments = new object();
    static bool _jeuEnCours = true;

    static async Task Main(string[] args)
    {
        _colony.Buildings.BuildingStacks.Add(new Mine(ResourceTypeEnums.Fer) { BuildingType = BuildingType.IronMine });
        GameUI ui = new GameUI(_colony);
        
        var tacheMoteur = Task.Run(() => BoucleDeJeu());

        while (_jeuEnCours)
        {
            string chosenAction = await ui.ShowDashboard();

            if (_jeuEnCours)
            {
                ui.ClearScreen();
                ExecuterActionMenu(chosenAction, ui);
            }
        }

        await tacheMoteur;
    }

    static void ExecuterActionMenu(string choix, GameUI ui)
    {
        switch (choix)
        {
            case "Voir mes Bâtiments": ui.AfficherBatiments(_verrouBatiments); break;
            case "Construire un Bâtiment": ui.MenuConstruction(_verrouBatiments); break;
            case "Améliorer un Bâtiment": ui.MenuAmelioration(_verrouBatiments); break;
            case "Gérer Population (Debug)": ui.MenuPopulation(); break;
            case "Quitter": _jeuEnCours = false; break;
        }
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
                            _colony.Resources.Ajouter(ResourceTypeEnums.Patate, productionNourriture);
                            break;

                        case BuildingType.IronMine:
                            int productionFer = (stack.Level * 2) * stack.Amount;
                            _colony.Resources.Ajouter(ResourceTypeEnums.Fer, productionFer);
                            break;
                    }
                }
            }

            int nb = _colony.Population.GetStock();
            if (nb > 0)
            {
                if (_colony.Resources.HasEnough(ResourceTypeEnums.Patate, nb)) _colony.Resources.Retirer(ResourceTypeEnums.Patate, nb);
                else _colony.Population.Retirer(1);
            }
            await Task.Delay(1000);
        }
    }
}