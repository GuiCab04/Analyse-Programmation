using AnalyseProgra.Models;
using AnalyseProgra.UserInterfaces;

namespace AnalyseProgra.Interactions.ColonyInteractions
{
    public class ShowBuildings : ColonyInteraction
    {


        public ShowBuildings(Colony colony)
            : base(colony, "Voir mes Bâtiments", "Affiche la liste de vos bâtiments.")
        {
        }

        public override void Execute(IUserInterface input)
        {
            var title = "Vos Bâtiments";
            var headers = new List<string> { "N°", "Nom", "Niveau", "Détails" };
            var table = new List<List<string>>();

            int n = 1;
            foreach (var b in _colony.Buildings.BuildingStacks)
            {
                string color;
                if (b.Level >= 5)
                    color = "green";
                else if (b.Level >= 3)
                    color = "yellow";
                else
                    color = "red";
                for (var i = 0; i < b.Amount; i++) {
                    table.Add(new List<string> { n.ToString(), $"[{color}]{b.BuildingType}[/]", b.Level.ToString(), "Voir Détails" });
                    n++;
                }
            }

            input.DisplayTable(title, headers, table);
            input.Pause();
        }
    }
}
