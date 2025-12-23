using AnalyseProgra.Models.Users;
using AnalyseProgra.UserInterfaces;
using AnalyseProgra.Views;
using AnalyseProgra.Controllers;

namespace testSpectre
{
    public class Program
    {
        static void Main(string[] args)
        {
            IUserInterface ui = new PlayerUI();

            var users = new List<User>
            {
                new Player("Player1"),
                new Player("Player2")
            };


            // Sélection du User
            User currentUser = ui.Select("Qui êtes-vous ?", users, u => u.Name);

            // --- BOUCLE DE JEU ---
            while (true)
            {
                // 1. AFFICHER LE DASHBOARD (Rafraîchissement)
                ui.ShowDashboard(currentUser);

                // 2. RECUPERER LES ACTIONS
                var actions = currentUser.AvalableActions;

                // 3. CHOIX DE L'ACTION
                // Le dashboard reste affiché au-dessus pendant que l'user choisit
                Interaction actionChoisie = ui.Select("Que voulez-vous faire ?", actions);

                // 4. EXECUTION
                try
                {
                    actionChoisie.Execute(ui);
                }
                catch (Exception ex)
                {
                    ui.WriteError(ex.Message);
                }

                // 5. PAUSE POUR LIRE LE RESULTAT
                // Comme on fait un AnsiConsole.Clear() au début de la boucle (dans ShowDashboard),
                // il faut laisser le temps au joueur de lire le résultat de son action.
                ui.WriteMessage("\n[grey]Appuyez sur une touche pour continuer...[/]");
                Console.ReadKey(true);
            }
        }
    }
}