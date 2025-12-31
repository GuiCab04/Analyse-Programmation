using AnalyseProgra.Models;
using AnalyseProgra.Models.Buildings;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Core.Managers
{
    public class BuildingManager
    {
        public List<ColonyBuildingStack> BuildingStacks { get; set; }

        public BuildingManager(ICollection<ColonyBuildingStack>? buildingStacks = null)
        {
            BuildingStacks = buildingStacks != null ? buildingStacks.ToList() : new List<ColonyBuildingStack>();
            
        }

        public static ColonyBuildingStack BuildBuildingStack(BuildingType type, int level)
        {
            switch (type)
            {
                case BuildingType.Farm:
                    return new Ferme(level);
                case BuildingType.HousingBuilding:
                    return new HousingBuilding(level);
                case BuildingType.IronMine:
                    return new Mine(level, ResourceTypeEnums.Fer);
                case BuildingType.GoldMine:
                    return new Mine(level, ResourceTypeEnums.Or);
                case BuildingType.StorageBuilding:
                     return new StorageBuilding(level);
                default:
                    throw new InvalidOperationException("Unknown BuildingType");
            }
        }

        public void AddBuilding(BuildingType type, int level)
        {
            if (!BuildingStacks.Any(b => b.BuildingType == type && b.Level == level))
            {
                var newStack = BuildBuildingStack(type, level);
                newStack.Amount = 1;
                BuildingStacks.Add(newStack);
            }
            else
            {
                var existingStack = BuildingStacks.First(b => b.BuildingType == type && b.Level == level);
                existingStack.Amount += 1;
            }
        }

        public void RemoveBuilding(BuildingType type, int level)
        {
            var stack = BuildingStacks.FirstOrDefault(b => b.BuildingType == type && b.Level == level);
            if (stack != null)
            {
                if (stack.Amount > 1)
                {
                    stack.Amount -= 1;
                }
                else
                {
                    BuildingStacks.Remove(stack);
                }
            }
            else
            {
                throw new InvalidOperationException("No building of the specified type and level exists to remove.");
            }
        }

        public void UpgradeBuilding(BuildingType type, int currentLevel)
        {
            try
            {
                RemoveBuilding(type, currentLevel);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Upgrade failed: No building of the specified type and level to upgrade.");
            }

            AddBuilding(type, currentLevel + 1);

        }
    }
}
