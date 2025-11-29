using System.Collections.Generic;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public class Ferme : ProductionBuilding
    {
        
        public Ferme(int id)
            : base(id, "Ferme", ResourceTypeEnums.Patate)
        {
        }

        public override Dictionary<ResourceTypeEnums, int> GetUpgradeCost()
        {            
            return new Dictionary<ResourceTypeEnums, int>
            {
                { ResourceTypeEnums.Fer, Level * 30 }
            };
        }
    }
}