using AnalyseProgra.Models;

namespace AnalyseProgra.Core.Managers
{
    public class BuildingManager
    {
        public List<ColonyBuildingStack> BuildingStacks { get; set; }

        public BuildingManager(ICollection<ColonyBuildingStack>? buildingStacks = null)
        {
            BuildingStacks = buildingStacks != null ? buildingStacks.ToList() : new List<ColonyBuildingStack>();
            
        }
    }
}
