using AnalyseProgra.Models.Enums;

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
        public string Name { get; set; } = "";


        public ColonyBuildingStack(string name, int level, int amount)
        {
            Name = name;
            Level = level;
            Amount = amount;
        }

        public ColonyBuildingStack(string name)
            : this(name, 1, 1)
        {
        }

        public abstract ICollection<ColonyResource>? GetUpgradeCost();
    }
}
