using AnalyseProgra.Models.Enums;
using AnalyseProgra.Core.Managers;

namespace AnalyseProgra.Models
{
    public abstract class ColonyBuildingStack
    {
        // Database fields
        public int ColonyId { get; set; }
        public BuildingType BuildingType { get; set; } // <-- enum
        public int Level { get; set; }
        public int Amount { get; set; }

        // Additional properties
        public string Name
        {
            get
            {
                return BuildingManager.GetBuildingName(BuildingType);
            }
        }

        public ColonyBuildingStack(BuildingType type, int level, int amount = 1)
        {
            BuildingType = type;
            Level = level;
            Amount = amount;
        }

        public abstract ICollection<ColonyResource>? GetUpgradeCost();
    }
}
