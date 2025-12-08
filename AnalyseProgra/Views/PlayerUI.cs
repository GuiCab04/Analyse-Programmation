using Spectre.Console;
using AnalyseProgra.Models.Users;

namespace AnalyseProgra.Views
{
    public class PlayerUI : SpectreInterface
    {
        public override void ShowDashboard(User user)
        {
            Player player;
            try
            {
                player = (Player)user;
            } catch (InvalidCastException)
            {
                WriteError("L'utilisateur fourni n'est pas un joueur valide.");
                return;
            }

            AnsiConsole.Clear();

            WriteTitle($"Tableau de bord de {player.Name}");

            var table = new Table();
            table.AddColumn("Attribut");
            table.AddColumn("Valeur");

            table.AddRow("Population", player.Population.ToString());
            table.AddRow("Nombre de bâtiments", player.Buildings.Count.ToString());

            AnsiConsole.Write(table);
        }
    }
}
