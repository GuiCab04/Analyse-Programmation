using System.Collections.Generic;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public class Ferme : ProductionBuilding
    {
        public Ferme(int level)
            : base("Ferme", BuildingType.Farm, level, ResourceTypeEnums.Patate)
        {
        }

        public override ICollection<ColonyResource>? GetUpgradeCost()
        {
            return new List<ColonyResource>
            {
                new ColonyResource(ResourceTypeEnums.Fer, Level * 75),
            };
        }
    }
}