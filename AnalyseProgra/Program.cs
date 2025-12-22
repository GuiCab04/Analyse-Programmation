using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Rendering;

class Program
{
    // --- GESTIONNAIRES ---
    static ResourceManager _ressources = new ResourceManager();
    static PopulationManager _population = new PopulationManager();
    static object _verrouBatiments = new object();
    static List<Building> _batiments = new List<Building>();
    static bool _jeuEnCours = true;

    // --- ETAT DU MENU ---
    // On définit les options ici pour pouvoir les afficher dans la boucle Live
    static string[] _menuOptions = {
        "Voir mes Bâtiments",
        "Construire un Bâtiment",
        "Améliorer un Bâtiment",
        "Gérer Population (Debug)",
        "Quitter"
    };
    static int _menuSelection = 0; // L'index de l'option sélectionnée actuellement

    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // 1. INITIALISATION
        _ressources.Ajouter(ResourceTypeEnums.Fer, 50);
        lock (_verrouBatiments)
        {
            _batiments.Add(new Mine(1, ResourceTypeEnums.Fer));
            _batiments.Add(new Ferme(2));
            _batiments.Add(new HousingBuilding(3));
        }
        _population.UpdateMaxPopulation(_batiments);

        // 2. MOTEUR
        var tacheMoteur = Task.Run(() => BoucleDeJeu());

        // 3. BOUCLE PRINCIPALE
        while (_jeuEnCours)
        {
            // On lance l'interface Live. Elle rendra la main quand l'utilisateur appuiera sur ENTRÉE.
            await AfficherInterfaceUnifiee();

            // Si on est sorti de la fonction, c'est qu'une action a été validée.
            if (_jeuEnCours)
            {
                ExecuterActionMenu();
            }
        }

        await tacheMoteur;
    }

    // --- INTERFACE LIVE UNIFIÉE ---

    static async Task AfficherInterfaceUnifiee()
    {
        // Layout Global : Entête en haut, Corps en dessous
        var layout = new Layout("Root").SplitRows(new Layout("Header").Size(6), new Layout("Body"));

        // Le Corps est divisé en deux colonnes : Dashboard (Gauche) et Menu (Droite)
        layout["Body"].SplitColumns(
            new Layout("Dashboard"),
            new Layout("Menu").Size(30) // Menu largeur fixe
        );

        await AnsiConsole.Live(layout)
            .AutoClear(true) // Nettoie l'écran quand on part dans un sous-menu
            .Overflow(VerticalOverflow.Ellipsis)
            .StartAsync(async ctx =>
            {
                bool actionValidee = false;

                while (!actionValidee && _jeuEnCours)
                {
                    // 1. MISE A JOUR VISUELLE
                    layout["Header"].Update(CreerEntete());
                    layout["Dashboard"].Update(CreerTableauDeBord());
                    layout["Menu"].Update(CreerMenuVisuel()); // On dessine le menu nous-même

                    ctx.Refresh();

                    // 2. GESTION CLAVIER (Navigation Menu)
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true).Key;

                        if (key == ConsoleKey.UpArrow)
                        {
                            _menuSelection--;
                            if (_menuSelection < 0) _menuSelection = _menuOptions.Length - 1; // Boucle vers la fin
                        }
                        else if (key == ConsoleKey.DownArrow)
                        {
                            _menuSelection++;
                            if (_menuSelection >= _menuOptions.Length) _menuSelection = 0; // Boucle vers le début
                        }
                        else if (key == ConsoleKey.Enter)
                        {
                            actionValidee = true; // On sort de la boucle Live pour exécuter l'action
                        }
                    }

                    await Task.Delay(50); // Fluidité
                }
            });
    }

    // --- WIDGETS ---

    static IRenderable CreerEntete()
    {
        return new Panel(
            new FigletText("PLANET COLONY").Color(Color.Cyan1).LeftJustified())
            .Border(BoxBorder.None);
    }

    static IRenderable CreerTableauDeBord()
    {
        var grid = new Grid().Expand();
        grid.AddColumn();

        // --- POPULATION ---
        var popActuelle = _population.GetStock();
        var panelPop = new Panel(
            Align.Center(new Markup($"[bold yellow]{popActuelle}[/] Habitants  -  [dim]Moral: Stable[/]")))
            .Header("Population")
            .BorderColor(Color.Green);

        // --- RESSOURCES ---
        var tableRes = new Table().Border(TableBorder.Rounded).Expand();

        tableRes.AddColumn(new TableColumn("Ressource").Width(15).NoWrap());
        tableRes.AddColumn(new TableColumn("Stock").Width(20).RightAligned());
        tableRes.AddColumn("Jauge");

        // CALCUL DE LA LARGEUR DISPONIBLE POUR LA JAUGE
        // Largeur Ecran 
        // - (Largeur Menu 30) 
        // - (Col Ressource 15) 
        // - (Col Stock 20) 
        // - (Marge de sécurité pour Bordures Tableau + Panel + Padding : ~17)
        int largeurFenetre = AnsiConsole.Profile.Width;
        int largeurFixeColonnes = 30 + 15 + 20 + 17; // Total 82

        int totalWidth = Math.Max(0, largeurFenetre - largeurFixeColonnes);

        foreach (ResourceTypeEnums type in Enum.GetValues(typeof(ResourceTypeEnums)))
        {
            int stock = _ressources.GetStock(type);
            int max = _ressources.GetMax(type);

            double ratio = max > 0 ? (double)stock / max : 0;

            int filled = (int)(ratio * totalWidth);
            filled = Math.Clamp(filled, 0, totalWidth);
            int empty = totalWidth - filled;

            string color = ratio > 0.9 ? "red" : "blue";
            string barVisual = $"[{color}]{new string('█', filled)}[/]{new string(' ', empty)}";

            tableRes.AddRow(
                $"[bold]{type}[/]",
                $"{stock}/{max}",
                barVisual
            );
        }

        grid.AddRow(panelPop);
        grid.AddRow(tableRes);
        return new Panel(grid).Header("Tableau de Bord").BorderColor(Color.Blue);
    }

    static IRenderable CreerMenuVisuel()
    {
        var list = new List<Markup>();

        for (int i = 0; i < _menuOptions.Length; i++)
        {
            if (i == _menuSelection)
            {
                // Option sélectionnée : En couleur et avec un curseur >
                list.Add(new Markup($"[bold yellow]> {_menuOptions[i]}[/]"));
            }
            else
            {
                // Option normale : Grisée
                list.Add(new Markup($"[grey]  {_menuOptions[i]}[/]"));
            }
        }

        // On met tout dans une Grid ou des Rows
        var rows = new Rows(list);

        return new Panel(rows)
            .Header("Actions")
            .BorderColor(Color.Yellow)
            .Expand();
    }

    // --- LOGIQUE ACTIONS ---

    static void ExecuterActionMenu()
    {
        string choix = _menuOptions[_menuSelection];
        AnsiConsole.Clear();

        switch (choix)
        {
            case "Voir mes Bâtiments": AfficherBatiments(); break;
            case "Construire un Bâtiment": MenuConstruction(); break;
            case "Améliorer un Bâtiment": MenuAmelioration(); break;
            case "Gérer Population (Debug)": MenuPopulation(); break;
            case "Quitter": _jeuEnCours = false; break;
        }
    }

    static void AfficherBatiments()
    {
        var table = new Table().Title("Vos Installations");
        table.AddColumn("Index");
        table.AddColumn("Nom");
        table.AddColumn("Niveau");
        table.AddColumn("Détails");

        lock (_verrouBatiments)
        {
            int index = 0;
            foreach (var b in _batiments)
            {
                string details = "";
                string color = "white";

                if (b is ProductionBuilding prod) { details = $"Prod: [green]+{prod.TauxProduction}[/] {prod.ResourceProduite}/s"; color = "yellow"; }
                else if (b is HousingBuilding house) { details = $"Lits: [blue]{house.CapaciteHabitants}[/]"; color = "cyan"; }
                else if (b is StorageBuilding storage)
                {
                    string type = storage.TypeStockage.HasValue ? storage.TypeStockage.ToString() : "Global";
                    details = $"Stock: [purple]+{storage.CapaciteAjoutee}[/] ({type})"; color = "grey";
                }

                table.AddRow(index.ToString(), $"[{color}]{b.Nom}[/]", b.Level.ToString(), details);
                index++;
            }
        }
        AnsiConsole.Write(table);
        Pause();
    }

    static void MenuConstruction()
    {
        // Menu classique pour la construction (car trop complexe à simuler en Live simple)
        var choix = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Que voulez-vous [green]construire[/] ?")
                .PageSize(10)
                .AddChoices(new[] {
                    "Mine de Fer (50 Fer)", "Mine d'Or (150 Fer)", "Ferme (30 Fer)",
                    "Maison (50 Fer)", "Silo Patates (80 Fer)", "Hangar Fer (100 Fer)",
                    "[red]Retour[/]"
                }));

        if (choix == "[red]Retour[/]") return;

        int newId;
        lock (_verrouBatiments) { newId = (_batiments.Count > 0 ? _batiments.Max(b => b.Id) : 0) + 1; }
        bool succes = false;

        if (choix.Contains("Mine de Fer")) succes = TryBuild(50, new Mine(newId, ResourceTypeEnums.Fer));
        else if (choix.Contains("Mine d'Or")) succes = TryBuild(150, new Mine(newId, ResourceTypeEnums.Or));
        else if (choix.Contains("Ferme")) succes = TryBuild(30, new Ferme(newId));

        else if (choix.Contains("Maison"))
        {
            succes = TryBuild(50, new HousingBuilding(newId));
            if (succes) _population.UpdateMaxPopulation(_batiments);
        }
        else if (choix.Contains("Silo Patates"))
        {
            succes = TryBuild(80, new StorageBuilding(newId, ResourceTypeEnums.Patate));
            if (succes) _ressources.UpdateMaxStorage(_batiments);
        }
        else if (choix.Contains("Hangar Fer"))
        {
            succes = TryBuild(100, new StorageBuilding(newId, ResourceTypeEnums.Fer));
            if (succes) _ressources.UpdateMaxStorage(_batiments);
        }

        if (succes)
        {
            AnsiConsole.Status().Start("Construction...", ctx => { ctx.Spinner(Spinner.Known.Clock); System.Threading.Thread.Sleep(1500); });
            AnsiConsole.MarkupLine("[green bold]Construction terminée ![/]");
        }
        else AnsiConsole.MarkupLine("[red bold]Pas assez de ressources ![/]");

        Pause();
    }

    static void MenuAmelioration()
    {
        List<string> choices;
        lock (_verrouBatiments)
        {
            if (_batiments.Count == 0) { AnsiConsole.MarkupLine("[red]Aucun bâtiment.[/]"); Pause(); return; }
            choices = _batiments.Select((b, i) => $"{i} - {b.Nom} (Niv {b.Level})").ToList();
        }
        choices.Add("[red]Retour[/]");

        var selection = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Améliorer ?").AddChoices(choices));
        if (selection == "[red]Retour[/]") return;

        int index = int.Parse(selection.Split('-')[0].Trim());
        Building batiment;
        lock (_verrouBatiments) { batiment = _batiments[index]; }

        var couts = batiment.GetUpgradeCost();
        AnsiConsole.MarkupLine($"Coût pour Niveau {batiment.Level + 1}:");
        foreach (var c in couts) AnsiConsole.MarkupLine($"- {c.Value} {c.Key}");

        if (AnsiConsole.Confirm("Confirmer ?"))
        {
            if (batiment.TryUpgrade(_ressources))
            {
                _population.UpdateMaxPopulation(_batiments);
                _ressources.UpdateMaxStorage(_batiments);
                AnsiConsole.MarkupLine("[green]Succès ![/]");
            }
            else AnsiConsole.MarkupLine("[red]Pas assez de ressources.[/]");
        }
        Pause();
    }

    static void MenuPopulation()
    {
        var action = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(new[] { "Ajouter Colon", "Tuer Colon", "Retour" }));
        if (action == "Ajouter Colon")
        {
            int avant = _population.GetStock(); _population.Ajouter(1);
            if (_population.GetStock() > avant) AnsiConsole.MarkupLine("[green]+1[/]"); else AnsiConsole.MarkupLine("[yellow]Manque de lits[/]");
        }
        else if (action == "Tuer Colon") { _population.Retirer(1); AnsiConsole.MarkupLine("[red]-1[/]"); }
    }

    static bool TryBuild(int cost, Building b)
    {
        if (_ressources.HasEnough(ResourceTypeEnums.Fer, cost))
        {
            _ressources.Retirer(ResourceTypeEnums.Fer, cost);
            lock (_verrouBatiments) { _batiments.Add(b); }
            return true;
        }
        return false;
    }

    static void Pause() { AnsiConsole.MarkupLine("[grey]Appuyez sur une touche...[/]"); Console.ReadKey(true); }

    // --- MOTEUR DE JEU ---
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