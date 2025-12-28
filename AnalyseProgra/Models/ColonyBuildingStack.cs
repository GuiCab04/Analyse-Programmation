using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models
{
    public class ColonyBuildingStack
    {
        public int ColonyId { get; set; }
        public BuildingType BuildingType { get; set; } // <-- enum
        public int Level { get; set; }
        public int Amount { get; set; }

        public Colony? Colony { get; set; }
    }
}
