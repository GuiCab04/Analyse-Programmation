using Spectre.Console;
using AnalyseProgra.Models.Enums;
using Spectre.Console.Rendering;
using AnalyseProgra.Models.Users;
using AnalyseProgra.Interactions;

namespace AnalyseProgra.Views
{
    public class PlayerUI : SpectreInterface
    {
        private Player _player;

        public PlayerUI(Player player) : base()
        {
            _player = player;
        }

        public async Task<Interaction> ShowDashboard()
        {
            List<string> menuOptions = new List<string>();
            _player.AvalableActions.ForEach(a => menuOptions.Add(a.Name));

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
                        layout["Menu"].Update(CreerMenuVisuel(menuOptions.ToArray(), menuSelection)); // On dessine le menu nous-même

                        ctx.Refresh();

                        // 2. GESTION CLAVIER (Navigation Menu)
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(true).Key;

                            if (key == ConsoleKey.UpArrow)
                            {
                                menuSelection--;
                                if (menuSelection < 0) menuSelection = menuOptions.Count - 1; // Boucle vers la fin
                            }
                            else if (key == ConsoleKey.DownArrow)
                            {
                                menuSelection++;
                                if (menuSelection >= menuOptions.Count) menuSelection = 0; // Boucle vers le début
                            }
                            else if (key == ConsoleKey.Enter)
                            {
                                actionValidee = true; // On sort de la boucle Live pour exécuter l'action
                            }
                        }

                        await Task.Delay(50); // Fluidité
                    }
                });
            return _player.AvalableActions[menuSelection];
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

            var popActuelle = _player.Colony.Population.CurrentPopulation;
            var popMax = _player.Colony.Population.MaxPopulation;
            double currentMoral = _player.Colony.Morale;

            string couleurMoral = currentMoral >= 75 ? "green" : (currentMoral >= 40 ? "yellow" : "red");
            var panelPop = new Panel(
                Align.Center(new Markup(
                    $"[bold yellow]{popActuelle}/{popMax}[/] Habitants  -  Moral: [{couleurMoral}]{currentMoral:0}%[/]"
                )))
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

            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                int stock = _player.Colony.Resources.GetStock(type);
                int max = _player.Colony.Resources.GetMax(type);

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