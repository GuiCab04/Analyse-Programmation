using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models;
using AnalyseProgra.Views;
using AnalyseProgra.DataAccess.Dao;
using AnalyseProgra.Models.Buildings;

class Program
{
    static Colony _colony = new Colony();
    static ResourceManager _ressources = new ResourceManager(_colony);
    static PopulationManager _population = new PopulationManager(_colony);
    static object _verrouBatiments = new object();
    static bool _jeuEnCours = true;

    static async Task Main(string[] args)
    {
        _colony.BuildingStacks.Add(new Mine(ResourceTypeEnums.Fer) { BuildingType = BuildingType.IronMine });
        PlayerUI ui = new PlayerUI(_ressources, _population, _colony.BuildingStacks);
        
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

    static void ExecuterActionMenu(string choix, PlayerUI ui)
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
            _population.UpdateMaxPopulation();
            _ressources.UpdateMaxStorage();

            if (_colony.BuildingStacks != null)
            {
                foreach (var stack in _colony.BuildingStacks)
                {
                    switch (stack.BuildingType)
                    {
                        case BuildingType.Farm:
                            int productionNourriture = (stack.Level * 5) * stack.Amount;
                            _ressources.Ajouter(ResourceTypeEnums.Patate, productionNourriture);
                            break;

                        case BuildingType.IronMine:
                            int productionFer = (stack.Level * 2) * stack.Amount;
                            _ressources.Ajouter(ResourceTypeEnums.Fer, productionFer);
                            break;
                    }
                }
            }

            int nb = _population.GetStock();
            if (nb > 0)
            {
                if (_ressources.HasEnough(ResourceTypeEnums.Patate, nb)) _ressources.Retirer(ResourceTypeEnums.Patate, nb);
                else _population.Retirer(1);
            }
            await Task.Delay(1000);
        }
    }
}