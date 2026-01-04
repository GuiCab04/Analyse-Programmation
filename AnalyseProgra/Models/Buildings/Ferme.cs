using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public class Ferme : ProductionBuilding
    {
        public Ferme(int level)
            : base(BuildingType.Farm, level, ResourceType.Solurial)
        {
        }

        public override ICollection<ColonyResource>? GetUpgradeCost()
        {
			switch (Level)
			{
				case 1:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 35),
						new ColonyResource(ResourceType.Soluro, 35),
						new ColonyResource(ResourceType.Solu, 10),
					};
				case 2:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 100),
						new ColonyResource(ResourceType.Soluro, 100),
						new ColonyResource(ResourceType.Solu, 60),
						new ColonyResource(ResourceType.Soli, 10),
					};
				case 3:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 300),
						new ColonyResource(ResourceType.Soluro, 300),
						new ColonyResource(ResourceType.Solu, 150),
						new ColonyResource(ResourceType.Soli, 75),
					};
				default:
					return new List<ColonyResource>();
			}
		}
	}
}