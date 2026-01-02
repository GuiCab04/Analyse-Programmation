using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public class Ferme : ProductionBuilding
    {
        public Ferme(int level)
            : base(BuildingType.Farm, level, ResourceType.Patate)
        {
        }

        public override ICollection<ColonyResource>? GetUpgradeCost()
        {
            return new List<ColonyResource>
            {
                new ColonyResource(ResourceType.Fer, Level * 75),
            };
        }
    }
}