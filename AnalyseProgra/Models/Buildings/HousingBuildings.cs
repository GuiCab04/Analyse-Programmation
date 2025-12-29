using System.Collections.Generic;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public class HousingBuilding : ColonyBuildingStack
    {
        public int CapaciteHabitants { get; private set; }

        public HousingBuilding() : base("Module d'Habitation")
        {
            CalculateCapacity();
        }

        public void UpdateGlobalPopulationLimit(PopulationManager popManager)
        {
            // popManager.AugmenterMax(CapaciteHabitants);
            // Cette méthode devra être créée dans PopulationManager
        }

        private void CalculateCapacity()
        {
            CapaciteHabitants = Level * 4;
        }

        public override ICollection<ColonyResource>? GetUpgradeCost()
        {
            return new List<ColonyResource>
            {
                new ColonyResource(ResourceTypeEnums.Fer, Level * 25),
                new ColonyResource(ResourceTypeEnums.Patate, Level * 50)
            };
        }
    }
}