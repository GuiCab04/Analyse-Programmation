using System.Collections.Generic;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public class Mine : ProductionBuilding
    {
        public Mine(int level, ResourceTypeEnums typeMinerai)
            : base($"Mine de {typeMinerai}",
                  typeMinerai == ResourceTypeEnums.Fer ? BuildingType.IronMine : BuildingType.GoldMine,
                  level,
                  typeMinerai)
        {
        }

        public override ICollection<ColonyResource>? GetUpgradeCost()
        {
            if (ResourceProduite == ResourceTypeEnums.Or)
            {
                // Une mine d'Or coûte cher et demande du Fer !
                return new List<ColonyResource>
                {
                    new ColonyResource(ResourceTypeEnums.Fer, Level * 200),
                };
            }
            else
            {
                // Une mine de Fer coûte un peu de Fer
                return new List<ColonyResource>
                {
                    new ColonyResource(ResourceTypeEnums.Fer, Level * 50),
                };
            }
        }
    }
}