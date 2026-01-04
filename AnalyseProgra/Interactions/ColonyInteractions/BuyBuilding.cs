using AnalyseProgra.Core.Managers;
using AnalyseProgra.Models;
using AnalyseProgra.Models.Buildings;
using AnalyseProgra.Models.Enums;
using AnalyseProgra.UserInterfaces;

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

            var typesConstructibles = new List<BuildingType>();

            foreach (BuildingType buildingType in Enum.GetValues(typeof(BuildingType)))
            {
                
                if (buildingType == BuildingType.Mairie)
                    continue;

                typesConstructibles.Add(buildingType);

                var buildingName = BuildingManager.GetBuildingName(buildingType);
                var requiredResources = BuildingManager.GetBaseCost(buildingType);
                string buildingCost = ResourceManager.ResourceListToString(requiredResources);

                choices.Add($"{buildingName}: {buildingCost}");
            }

            choices.Add("[red]Retour[/]");

            var choix = input.Select("Que voulez-vous [green]construire[/] ?", choices);

            if (choix == "[red]Retour[/]") return;

            int indexChoisi = choices.IndexOf(choix);

            BuildingType selectedBuildingType = typesConstructibles[indexChoisi];

            var cost = BuildingManager.GetBaseCost(selectedBuildingType);

            if (TryBuild(cost, selectedBuildingType, input))
            {
                input.Load(1000);
                input.WriteMessage($"[green bold]Construction de {BuildingManager.GetBuildingName(selectedBuildingType)} terminée ![/]");
            }

            input.Pause();
        }

        private bool TryBuild(ICollection<ColonyResource> requiredResources, BuildingType type, IUserInterface input)
		{
			var buildingManager = _colony.Buildings;
			var mairie = buildingManager.BuildingStacks.FirstOrDefault(b => b.BuildingType == BuildingType.Mairie) as Mairie;

			if (type == BuildingType.Mairie)
			{
				if (mairie != null)
				{
					input.WriteMessage("[red]Impossible : il existe déjà une mairie dans la colonie.");
					return false;
				}
			}

			if (mairie != null && type != BuildingType.Mairie)
			{
				int currentTotalBuildings = buildingManager.BuildingStacks.Where(b => b.BuildingType != BuildingType.Mairie).Sum(b => b.Amount);
				int allowed = mairie.GetMaxBuildingsAllowed();

				if (currentTotalBuildings + 1 > allowed)
				{
					input.WriteMessage($"[red]Impossible : la Mairie limite le nombre total de bâtiments. Améliorer la pour avoir la possibilité de construire plus de bâtiment.[/]");
					return false;
				}
			}

			if (!_colony.Resources.HasEnough(requiredResources))
			{
				input.WriteMessage("[red]Pas assez de ressources ![/]");
				return false;
			}

			_colony.Resources.Retirer(requiredResources);
			_colony.Buildings.AddBuilding(type, 1);
			return true;
		}
	}
}
