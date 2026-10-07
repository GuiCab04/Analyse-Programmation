using System.Collections.Generic;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public class HousingBuilding : ColonyBuildingStack
    {
        public int CapaciteHabitants { get; private set; }

        public HousingBuilding(int level)
            : base(BuildingType.HousingBuilding, level)
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
            CapaciteHabitants = Level switch
            {
                1 => 1,
                2 => 2,
                3 => 5,
                4 => 7,
                5 => 10,
                _ => 10
            };
        }

        public override ICollection<ColonyResource>? GetUpgradeCost()
        {
            return new List<ColonyResource>
            {
                new ColonyResource(ResourceType.Soluro, Level * 25),
                new ColonyResource(ResourceType.Solurial, Level * 50)
            };
        }
    }
}