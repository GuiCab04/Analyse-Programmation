using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models;
using AnalyseProgra.Views;
using AnalyseProgra.DataAccess.Dao;

class Program
{
    // --- GESTIONNAIRES ---
    static Colony? Colony = new ColonyDao().GetById(1, true);
    static ResourceManager _ressources = new ResourceManager(Colony);
    static PopulationManager _population = new PopulationManager(Colony);
    static object _verrouBatiments = new object();
    static bool _jeuEnCours = true;

    static async Task Main(string[] args)
    {
        PlayerUI ui = new PlayerUI(_ressources, _population, Colony.BuildingStacks);
        // Init
        Colony.BuildingStacks.Add(new ColonyBuildingStack
        {
            BuildingType = BuildingType.Factory,
            Level = 50,
            Amount = 2,
            Colony = Colony
        });
        
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

            if (Colony.BuildingStacks != null)
            {
                foreach (var stack in Colony.BuildingStacks)
                {
                    switch (stack.BuildingType)
                    {
                        case BuildingType.Farm:
                            int productionNourriture = (stack.Level * 5) * stack.Amount;
                            _ressources.Ajouter(ResourceTypeEnums.Patate, productionNourriture);
                            break;

                        case BuildingType.Factory:
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