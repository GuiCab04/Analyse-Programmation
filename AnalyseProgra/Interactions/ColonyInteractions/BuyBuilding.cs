using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models;
using AnalyseProgra.UserInterfaces;
using AnalyseProgra.Core.Managers;

namespace AnalyseProgra.Interactions.ColonyInteractions
{
    public class BuyBuilding : ColonyInteraction
    {

        public BuyBuilding(Colony colony)
            : base(colony, "Acheter un bâtiment", "Acheter et construire un nouveau bâtiment dans la colonie.")
        {
        }

        public override void Execute(IUserInterface input)
        {
            var choices = new List<string>();
            foreach (BuildingType buildingType in Enum.GetValues(typeof(BuildingType)))
            {
                var buildingName = BuildingManager.GetBuildingName(buildingType);
                var requiredResources = BuildingManager.GetBaseCost(buildingType);
                string buildingCost = ResourceManager.ResourceListToString(requiredResources);

                choices.Add($"{buildingName}: {buildingCost}");
            }
            choices.Add("[red]Retour[/]");

            var choix = input.Select("Que voulez-vous [green]construire[/] ?", choices);

            if (choix == "[red]Retour[/]") return;

            BuildingType selectedBuildingType = (BuildingType)choices.IndexOf(choix);
            var cost = BuildingManager.GetBaseCost(selectedBuildingType);
            if (TryBuild(cost, selectedBuildingType))
            {
                input.Load(1000);
                input.WriteMessage("[green bold]Construction terminée ![/]");
            }
            else
            {
                input.WriteError("Pas assez de ressources !");
            }

            input.Pause();
        }

        // Nouvelle signature : On prend BuildingType au lieu de Building object
        private bool TryBuild(ICollection<ColonyResource> requiredResources, BuildingType type)
        {
            if (_colony.Resources.HasEnough(requiredResources))
            {
                _colony.Resources.Retirer(requiredResources);
                _colony.Buildings.AddBuilding(type, 1);
                return true;
            }
            return false;
        }
    }
}
