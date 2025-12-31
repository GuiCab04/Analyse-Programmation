using Spectre.Console;
using AnalyseProgra.Models.Enums;
using Spectre.Console.Rendering;
using AnalyseProgra.Models;
using AnalyseProgra.Models.Buildings;
using AnalyseProgra.Core.Managers;

namespace AnalyseProgra.Views
{
    public class GameUI : SpectreInterface
    {
        private ResourceManager _ressources;
        private PopulationManager _population;
        private BuildingManager _batiments;

        public GameUI(Colony colony) : base()
        {
            _ressources = colony.Resources;
            _population = colony.Population;
            _batiments = colony.Buildings;
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
                foreach (var b in _batiments.BuildingStacks)
                {
                    string color = "white";

                    table.AddRow(index.ToString(), $"[{color}]{b.BuildingType}[/]", b.Level.ToString());
                    index++;
                }
            }
            AnsiConsole.Write(table);
            Pause();
        }

        public void MenuConstruction(object verrouBatiments)
        {
            // On mappe les anciens noms vers le nouvel Enum
            var choix = Select("Que voulez-vous [green]construire[/] ?", new[] {
                    "Usine/Mine (50 Fer)",
                    "Ferme (30 Fer)",
                    "Maison (50 Fer)",
                    "Entrepôt (80 Fer)",
                    "[red]Retour[/]"
                });

            if (choix == "[red]Retour[/]") return;

            bool succes = false;

            // Logique adaptée : On passe le TYPE (Enum) et le COÛT
            if (choix.Contains("Usine")) succes = TryBuild(50, BuildingType.IronMine, verrouBatiments);
            else if (choix.Contains("Ferme")) succes = TryBuild(30, BuildingType.Farm, verrouBatiments);
            else if (choix.Contains("Maison"))
            {
                succes = TryBuild(50, BuildingType.HousingBuilding, verrouBatiments);
                // Mise à jour immédiate des max
                if (succes) _population.UpdateMaxPopulation(_batiments);
            }
            else if (choix.Contains("Entrepôt"))
            {
                succes = TryBuild(80, BuildingType.StorageBuilding, verrouBatiments);
                // Mise à jour immédiate des max
                if (succes) _ressources.UpdateMaxStorage(_batiments); // Rappel : UpdateMaxStorage n'a plus besoin d'arguments
            }

            if (succes)
            {
                AnsiConsole.Status().Start("Construction...", ctx => { ctx.Spinner(Spinner.Known.Clock); Thread.Sleep(1000); });
                WriteMessage("[green bold]Construction terminée ![/]");
            }
            else
            {
                WriteError("Pas assez de ressources !");
            }

            Pause();
        }

        // Nouvelle signature : On prend BuildingType au lieu de Building object
        private bool TryBuild(int cost, BuildingType type, object verrouBatiments)
        {
            if (_ressources.HasEnough(ResourceTypeEnums.Fer, cost))
            {
                _ressources.Retirer(ResourceTypeEnums.Fer, cost);

                lock (verrouBatiments)
                {
                    // 1. On cherche s'il existe déjà un stack de ce type (niveau 1 par défaut pour la construction)
                    var existingStack = _batiments.BuildingStacks.FirstOrDefault(b => b.BuildingType == type && b.Level == 1);

                    if (existingStack != null)
                    {
                        // On incrémente juste la quantité
                        existingStack.Amount++;
                    }
                    else
                    {
                        ColonyBuildingStack newColonyBuildingStack;
                        switch (type)
                        {
                            case BuildingType.Farm:
                                newColonyBuildingStack = new Ferme();
                                break;
                            case BuildingType.HousingBuilding:
                                newColonyBuildingStack = new HousingBuilding();
                                break;
                            case BuildingType.IronMine:
                                newColonyBuildingStack = new Mine(ResourceTypeEnums.Fer);
                                break;
                            case BuildingType.GoldMine:
                                newColonyBuildingStack = new Mine(ResourceTypeEnums.Or);
                                break;
                            case BuildingType.StorageBuilding:
                                newColonyBuildingStack = new StorageBuilding();
                                break;
                            default:
                                throw new InvalidOperationException("Unknown BuildingType");
                        }
                        
                        _batiments.BuildingStacks.Add(newColonyBuildingStack);
                    }
                }
                return true;
            }
            return false;
        }

        public void MenuAmelioration(object verrouBatiments)
        {
            List<ColonyBuildingStack> upgradeableStacks;

            lock (verrouBatiments)
            {
                if (_batiments == null || _batiments.BuildingStacks.Count == 0) { WriteError("Aucun bâtiment."); Pause(); return; }
                // On convertit en liste pour pouvoir indexer
                upgradeableStacks = _batiments.BuildingStacks.ToList();
            }

            // Création du menu de sélection
            var choices = upgradeableStacks.Select((b, i) => $"{i} - {b.BuildingType} (Niv {b.Level}) x{b.Amount}").ToList();
            choices.Add("[red]Retour[/]");

            var selection = Select("Quel groupe de bâtiments améliorer ?", choices);
            if (selection == "[red]Retour[/]") return;

            int index = int.Parse(selection.Split('-')[0].Trim());
            ColonyBuildingStack stackSelectionne = upgradeableStacks[index];

            // CALCUL DU COÛT (Logique simulée ici faute de classe objet)
            int coutBase = GetBaseCost(stackSelectionne.BuildingType);
            int coutUpgrade = coutBase * stackSelectionne.Level; // Exemple : Niv 1 -> 2 coûte 1xBase

            WriteMessage($"Coût pour passer ce stack au Niveau {stackSelectionne.Level + 1}:");
            WriteMessage($"- {coutUpgrade} Fer par bâtiment dans le stack");
            WriteMessage($"[dim](Note: Cela améliorera tout le stack d'un coup ou diviser le stack est complexe)[/]");

            if (Confirm($"Confirmer l'amélioration pour {coutUpgrade} Fer ?"))
            {
                if (_ressources.HasEnough(ResourceTypeEnums.Fer, coutUpgrade))
                {
                    _ressources.Retirer(ResourceTypeEnums.Fer, coutUpgrade);

                    lock (verrouBatiments)
                    {
                        // Amélioration simple : On monte le niveau du stack
                        // Attention : Dans un vrai jeu, on diviserait peut-être le stack si on veut en upgrader qu'un seul
                        stackSelectionne.Level++;
                    }

                    // Mise à jour des calculs
                    _population.UpdateMaxPopulation(_batiments); // Conversion ToList nécessaire pour l'ancienne signature
                    _ressources.UpdateMaxStorage(_batiments);

                    WriteMessage("[green]Succès ! Niveau augmenté.[/]");
                }
                else WriteError("Pas assez de ressources.");
            }
            Pause();
        }

        // Helper pour retrouver les prix (remplace les classes objets)
        private int GetBaseCost(BuildingType type)
        {
            return type switch
            {
                BuildingType.HousingBuilding => 50,
                BuildingType.Farm => 30,
                BuildingType.IronMine => 100,
                BuildingType.GoldMine => 200,
                BuildingType.StorageBuilding => 80,
                _ => 50
            };
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