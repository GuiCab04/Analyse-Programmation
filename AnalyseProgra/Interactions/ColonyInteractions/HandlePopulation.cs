using AnalyseProgra.Models;
using AnalyseProgra.UserInterfaces;

namespace AnalyseProgra.Interactions.ColonyInteractions
{
    public class HandlePopulation : ColonyInteraction
    {
        public HandlePopulation(Colony colony)
            : base(colony, "Gérer Population", "Ajouter ou retirer des colons de votre colonie.")
        {
        }

        public override void Execute(IUserInterface input)
        {
            var action = input.Select("Gérer Population", new[] { "Ajouter Colon", "Tuer Colon", "Retour" });

            if (action == "Ajouter Colon")
            {
                int avant = _colony.Population.GetStock();
                _colony.Population.Ajouter(1);

                if (_colony.Population.GetStock() > avant) input.WriteMessage("[green]+1[/]");
                else input.WriteMessage("[yellow]Manque de lits[/]");
            }
            else if (action == "Tuer Colon")
            {
                _colony.Population.Retirer(1);
                input.WriteError("-1 (Colon retiré)");
            }
        }
    }
}
