using AnalyseProgra.Models;
using AnalyseProgra.Models.Buildings;
using AnalyseProgra.Models.Enums;
using System.Reflection.Emit;

namespace AnalyseProgra.Core.Managers
{
    public class BuildingManager
    {
        public List<ColonyBuildingStack> BuildingStacks { get; set; }

        public BuildingManager(ICollection<ColonyBuildingStack>?     buildingStacks = null)
        {
            BuildingStacks = buildingStacks != null ? buildingStacks.ToList() : new List<ColonyBuildingStack>();
            
        }

        public static string GetBuildingName(BuildingType type)
        {
            return type switch
            {
				BuildingType.Mairie => "Mairie",
				BuildingType.HousingBuilding => "Maison",
				BuildingType.Farm => "Ferme à Solurial",
				BuildingType.SaliFarm => "Ferme de Sali",
				BuildingType.SoluroMine => "Carrière de Soluro",
				BuildingType.SoliMine => "Mine de Soli",
				BuildingType.SoluMine => "Mine de Solu",
				BuildingType.StorageSaliBuilding => "Entrepôt de Sali",
				BuildingType.StorageSolurialBuilding => "Entrepôt de Solurial",
				BuildingType.StorageSoluroBuilding => "Entrepôt de Soluro",
				BuildingType.StorageSoluBuilding => "Entrepôt de Solu",
				BuildingType.StorageSoliBuilding => "Entrepôt de Soli",
				_ => "Bâtiment inconnu"
            };
        }

        public static ICollection<ColonyResource> GetBaseCost(BuildingType type)
        {
            return type switch
            {
				BuildingType.HousingBuilding => new List<ColonyResource>
				{
					new ColonyResource(ResourceType.Soluro, 15),
					new ColonyResource(ResourceType.Sali, 15)
				},
				BuildingType.Farm => new List<ColonyResource>
				{
					new ColonyResource(ResourceType.Soluro, 5),
					new ColonyResource(ResourceType.Sali, 5)
				},
				BuildingType.SoluroMine => new List<ColonyResource>
				{
					new ColonyResource(ResourceType.Sali, 5)
				},
				BuildingType.SaliFarm => new List<ColonyResource>
				{
					new ColonyResource(ResourceType.Soluro, 5)
				},
				BuildingType.SoliMine => new List<ColonyResource>
				{
					new ColonyResource(ResourceType.Soluro, 80),
					new ColonyResource(ResourceType.Sali, 80),
					new ColonyResource(ResourceType.Solu, 30)
				},
				BuildingType.SoluMine => new List<ColonyResource>
				{
					new ColonyResource(ResourceType.Sali, 25),
					new ColonyResource(ResourceType.Soluro, 25)
				},
				BuildingType.StorageSaliBuilding => new List<ColonyResource>
				{
					new ColonyResource(ResourceType.Sali, 20),
					new ColonyResource(ResourceType.Soluro, 20)
				},
				BuildingType.StorageSolurialBuilding => new List<ColonyResource>
				{
					new ColonyResource(ResourceType.Sali, 20),
					new ColonyResource(ResourceType.Soluro, 20),
					new ColonyResource(ResourceType.Solurial, 20)
				},
				BuildingType.StorageSoluroBuilding => new List<ColonyResource>
				{
					new ColonyResource(ResourceType.Sali, 20),
					new ColonyResource(ResourceType.Soluro, 20)
				},
				BuildingType.StorageSoluBuilding => new List<ColonyResource>
				{
					new ColonyResource(ResourceType.Sali, 120),
					new ColonyResource(ResourceType.Soluro, 120),
					new ColonyResource(ResourceType.Solu, 60)
				},
				BuildingType.StorageSoliBuilding => new List<ColonyResource>
				{
					new ColonyResource(ResourceType.Sali, 350),
					new ColonyResource(ResourceType.Soluro, 350),
					new ColonyResource(ResourceType.Soli, 20)
				},
				_ => throw new InvalidOperationException("Unknown BuildingType")
            };
        }

		public static ColonyBuildingStack BuildBuildingStack(BuildingType type, int level)
		{
			switch (type)
			{
				case BuildingType.Farm:
					return new Ferme(level);
				case BuildingType.HousingBuilding:
					return new HousingBuilding(level);
				case BuildingType.SoluroMine:
					return new Mine(level, ResourceType.Soluro);
				case BuildingType.SaliFarm:
					return new Mine(level, ResourceType.Sali);
				case BuildingType.SoliMine:
					return new Mine(level, ResourceType.Soli);
				case BuildingType.SoluMine:
					return new Mine(level, ResourceType.Solu);
				case BuildingType.StorageSaliBuilding:
					return new StorageBuilding(level, ResourceType.Sali);
				case BuildingType.StorageSoluroBuilding:
					return new StorageBuilding(level, ResourceType.Soluro);
				case BuildingType.StorageSolurialBuilding:
					return new StorageBuilding(level, ResourceType.Solurial);
				case BuildingType.StorageSoluBuilding:
					return new StorageBuilding(level, ResourceType.Solu);
				case BuildingType.StorageSoliBuilding:
					return new StorageBuilding(level, ResourceType.Soli);
				case BuildingType.Mairie:
					return new Mairie(level);
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
			int targetLevel = currentLevel + 1;

			var mairieStack = BuildingStacks.FirstOrDefault(b => b.BuildingType == BuildingType.Mairie);
			if (mairieStack != null)
			{
				int mairieLevel = mairieStack.Level;
				if (targetLevel > mairieLevel)
				{
					throw new InvalidOperationException($"Impossible d'améliorer : améliorez d'abord la Mairie (niv {mairieLevel}) pour débloquer ce niveau.");
				}
			}
			try
			{
				RemoveBuilding(type, currentLevel);
			}
			catch (InvalidOperationException ex)
			{
				throw new InvalidOperationException("Upgrade failed: No building of the specified type and level to upgrade.");
			}

			AddBuilding(type, targetLevel);
		}
	}
}
