using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models.Buildings;
using AnalyseProgra.Views;

class Program
{
    // --- GESTIONNAIRES ---
    static ResourceManager _ressources = new ResourceManager();
    static PopulationManager _population = new PopulationManager();
    static object _verrouBatiments = new object();
    static List<Building> _batiments = new List<Building>();
    static bool _jeuEnCours = true;

    static async Task Main(string[] args)
    {
        PlayerUI ui = new PlayerUI(_ressources, _population, _batiments);
        // 1. INITIALISATION
        _ressources.Ajouter(ResourceTypeEnums.Fer, 50);
        lock (_verrouBatiments)
        {
            _batiments.Add(new Mine(ResourceTypeEnums.Fer));
            _batiments.Add(new Ferme());
            _batiments.Add(new HousingBuilding());
        }
        _population.UpdateMaxPopulation(_batiments);

        // 2. MOTEUR
        var tacheMoteur = Task.Run(() => BoucleDeJeu());

        // 3. BOUCLE PRINCIPALE
        while (_jeuEnCours)
        {
            // On lance l'interface Live. Elle rendra la main quand l'utilisateur appuiera sur ENTRÉE.
            string chosenAction = await ui.ShowDashboard();

            // Si on est sorti de la fonction, c'est qu'une action a été validée.
            if (_jeuEnCours)
            {
                ui.ClearScreen();
                ExecuterActionMenu(chosenAction, ui);
            }
        }

        await tacheMoteur;
    }

    // --- LOGIQUE ACTIONS ---

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
            List<Building> copieBatiments;
            lock (_verrouBatiments) { copieBatiments = _batiments.ToList(); }

            _population.UpdateMaxPopulation(copieBatiments);
            _ressources.UpdateMaxStorage(copieBatiments);

            foreach (var b in copieBatiments) { if (b is ProductionBuilding p) p.Produire(_ressources); }

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