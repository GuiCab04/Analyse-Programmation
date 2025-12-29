using System.Collections.Generic;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public class Ferme : ProductionBuilding
    {
        public Ferme()
            : base("Ferme", ResourceTypeEnums.Patate)
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