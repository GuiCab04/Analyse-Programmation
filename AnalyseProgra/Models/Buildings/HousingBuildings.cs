using System.Collections.Generic;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public class HousingBuilding : Building
    {
        public int CapaciteHabitants { get; private set; }

        public HousingBuilding(int id) : base(id, "Module d'Habitation")
        {
            CalculateCapacity();
        }

        public void UpdateGlobalPopulationLimit(PopulationManager popManager)
        {
            // popManager.AugmenterMax(CapaciteHabitants);
            // Cette méthode devra être créée dans PopulationManager
        }

        protected override void ApplyUpgradeEffect()
        {
            CalculateCapacity();
        }

        private void CalculateCapacity()
        {
            CapaciteHabitants = Level * 4;
        }

        public override Dictionary<ResourceTypeEnums, int> GetUpgradeCost()
        {
            return new Dictionary<ResourceTypeEnums, int>
            {
                { ResourceTypeEnums.Fer, Level * 25 },
                { ResourceTypeEnums.Patate, Level * 50 }
            };
        }
    }
}