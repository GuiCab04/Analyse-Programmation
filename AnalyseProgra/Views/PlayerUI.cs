using Spectre.Console;
using AnalyseProgra.Models.Users;
using AnalyseProgra.Models.Enums;
using Spectre.Console.Rendering;
using AnalyseProgra.Models.Buildings;

namespace AnalyseProgra.Views
{
    public class PlayerUI : SpectreInterface
    {
        private ResourceManager _ressources;
        private PopulationManager _population;
        private List<Building> _batiments;

        public PlayerUI(ResourceManager ressources, PopulationManager population, List<Building> batiments) : base()
        {
            _ressources = ressources;
            _population = population;
            _batiments = batiments;
        }

        public void AfficherBatiments(object verrouBatiments)
        {
            var table = new Table().Title("Vos Installations");
            table.AddColumn("Index");
            table.AddColumn("Nom");
            table.AddColumn("Niveau");
            table.AddColumn("Détails");

            lock (verrouBatiments)
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

        public void MenuConstruction(object verrouBatiments)
        {
            var choix = Select("Que voulez-vous [green]construire[/] ?", new[] {
                    "Mine de Fer (50 Fer)", "Mine d'Or (150 Fer)", "Ferme (30 Fer)",
                    "Maison (50 Fer)", "Silo Patates (80 Fer)", "Hangar Fer (100 Fer)",
                    "[red]Retour[/]"
                });

            if (choix == "[red]Retour[/]") return;

            bool succes = false;
            if (choix.Contains("Mine de Fer")) succes = TryBuild(50, new Mine(ResourceTypeEnums.Fer), verrouBatiments);
            else if (choix.Contains("Mine d'Or")) succes = TryBuild(150, new Mine(ResourceTypeEnums.Or), verrouBatiments);
            else if (choix.Contains("Ferme")) succes = TryBuild(30, new Ferme(), verrouBatiments);
            else if (choix.Contains("Maison"))
            {
                succes = TryBuild(50, new HousingBuilding(), verrouBatiments);
                if (succes) _population.UpdateMaxPopulation(_batiments);
            }
            else if (choix.Contains("Silo Patates"))
            {
                succes = TryBuild(80, new StorageBuilding(ResourceTypeEnums.Patate), verrouBatiments);
                if (succes) _ressources.UpdateMaxStorage(_batiments);
            }
            else if (choix.Contains("Hangar Fer"))
            {
                succes = TryBuild(100, new StorageBuilding(ResourceTypeEnums.Fer), verrouBatiments);
                if (succes) _ressources.UpdateMaxStorage(_batiments);
            }

            if (succes)
            {
                AnsiConsole.Status().Start("Construction...", ctx => { ctx.Spinner(Spinner.Known.Clock); Thread.Sleep(1500); });
                WriteMessage("[green bold]Construction terminée ![/]");
            }
            else
            {
                WriteError("Pas assez de ressources !");
            }

            Pause();
        }

        private bool TryBuild(int cost, Building b, object verrouBatiments)
        {
            if (_ressources.HasEnough(ResourceTypeEnums.Fer, cost))
            {
                _ressources.Retirer(ResourceTypeEnums.Fer, cost);
                lock (verrouBatiments) { _batiments.Add(b); }
                return true;
            }
            return false;
        }

        public void MenuAmelioration(object verrouBatiments)
        {
            List<string> choices;
            lock (verrouBatiments)
            {
                if (_batiments.Count == 0) { WriteError("Aucun bâtiment."); Pause(); return; }
                choices = _batiments.Select((b, i) => $"{i} - {b.Nom} (Niv {b.Level})").ToList();
            }
            choices.Add("[red]Retour[/]");

            var selection = Select("Améliorer ?", choices);
            if (selection == "[red]Retour[/]") return;

            int index = int.Parse(selection.Split('-')[0].Trim());
            Building batiment;
            lock (verrouBatiments) { batiment = _batiments[index]; }

            var couts = batiment.GetUpgradeCost();
            WriteMessage($"Coût pour Niveau {batiment.Level + 1}:");
            foreach (var c in couts) WriteMessage($"- {c.Value} {c.Key}");

            if (Confirm("Confirmer l'amélioration ?"))
            {
                if (batiment.TryUpgrade(_ressources))
                {
                    _population.UpdateMaxPopulation(_batiments);
                    _ressources.UpdateMaxStorage(_batiments);
                    WriteMessage("[green]Succès ![/]");
                }
                else WriteError("Pas assez de ressources.");
            }
            Pause();
        }

        public void MenuPopulation()
        {
            var action = Select("Gérer Population", new[] { "Ajouter Colon", "Tuer Colon", "Retour" });

            if (action == "Ajouter Colon")
            {
                int avant = _population.GetStock();
                _population.Ajouter(1);

                if (_population.GetStock() > avant) WriteMessage("[green]+1[/]");
                else WriteMessage("[yellow]Manque de lits[/]");
            }
            else if (action == "Tuer Colon")
            {
                _population.Retirer(1);
                WriteError("-1 (Colon retiré)");
            }
        }

        public async Task<string> ShowDashboard()
        {
            string[] menuOptions = {
                "Voir mes Bâtiments",
                "Construire un Bâtiment",
                "Améliorer un Bâtiment",
                "Gérer Population (Debug)",
                "Quitter"
            };
            int menuSelection = 0;

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

                    while (!actionValidee)
                    {
                        // 1. MISE A JOUR VISUELLE
                        layout["Header"].Update(CreerEntete());
                        layout["Dashboard"].Update(CreerTableauDeBord());
                        layout["Menu"].Update(CreerMenuVisuel(menuOptions, menuSelection)); // On dessine le menu nous-même

                        ctx.Refresh();

                        // 2. GESTION CLAVIER (Navigation Menu)
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(true).Key;

                            if (key == ConsoleKey.UpArrow)
                            {
                                menuSelection--;
                                if (menuSelection < 0) menuSelection = menuOptions.Length - 1; // Boucle vers la fin
                            }
                            else if (key == ConsoleKey.DownArrow)
                            {
                                menuSelection++;
                                if (menuSelection >= menuOptions.Length) menuSelection = 0; // Boucle vers le début
                            }
                            else if (key == ConsoleKey.Enter)
                            {
                                actionValidee = true; // On sort de la boucle Live pour exécuter l'action
                            }
                        }

                        await Task.Delay(50); // Fluidité
                    }
                });
            return menuOptions[menuSelection];
        }

        private IRenderable CreerEntete()
        {
            return new Panel(
                new FigletText("PLANET COLONY").Color(Color.Cyan1).LeftJustified())
                .Border(BoxBorder.None);
        }

        private IRenderable CreerTableauDeBord()
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

        private IRenderable CreerMenuVisuel(string[] menuOptions, int menuSelection)
        {
            var list = new List<Markup>();

            for (int i = 0; i < menuOptions.Length; i++)
            {
                if (i == menuSelection)
                {
                    // Option sélectionnée : En couleur et avec un curseur >
                    list.Add(new Markup($"[bold yellow]> {menuOptions[i]}[/]"));
                }
                else
                {
                    // Option normale : Grisée
                    list.Add(new Markup($"[grey]  {menuOptions[i]}[/]"));
                }
            }

            // On met tout dans une Grid ou des Rows
            var rows = new Rows(list);

            return new Panel(rows)
                .Header("Actions")
                .BorderColor(Color.Yellow)
                .Expand();
        }
    }
}