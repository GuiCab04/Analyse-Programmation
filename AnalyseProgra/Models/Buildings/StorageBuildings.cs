using System.Collections.Generic;
using System.Reflection;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
	public class StorageBuilding : ColonyBuildingStack
	{
		public int CapaciteAjoutee { get; private set; }

		public ResourceType? TypeStockage { get; private set; }

		public StorageBuilding(int level, ResourceType? typeSpecific = null)
			: base(typeSpecific switch
			{
				ResourceType.Sali => BuildingType.StorageSaliBuilding,
				ResourceType.Solurial => BuildingType.StorageSolurialBuilding,
				ResourceType.Soluro => BuildingType.StorageSoluroBuilding,
				ResourceType.Solu => BuildingType.StorageSoluBuilding,
				ResourceType.Soli => BuildingType.StorageSoliBuilding,
				_ => BuildingType.StorageSaliBuilding
			}, level)
		{
			TypeStockage = typeSpecific;
			CalculateCapacity();
		}

		private void CalculateCapacity()
		{
			switch (TypeStockage)
			{
				case ResourceType.Sali:
					CapaciteAjoutee = GetSaliStorageCapacity();
					break;
				case ResourceType.Solurial:
					CapaciteAjoutee = GetSolurialStorageCapacity();
					break;
				case ResourceType.Soluro:
					CapaciteAjoutee = GetSoluroStorageCapacity();
					break;
				case ResourceType.Solu:
					CapaciteAjoutee = GetSoluStorageCapacity();
					break;
				case ResourceType.Soli:
					CapaciteAjoutee = GetSoliStorageCapacity();
					break;
				default:
					CapaciteAjoutee = 0;
					break;
			}
		}

		private int GetSaliStorageCapacity()
		{
			switch (Level)
			{
				case 1: return 75;
				case 2: return 250;
				case 3: return 750;
				default: return 750; 
			}
		}

		private int GetSolurialStorageCapacity()
		{
			switch (Level)
			{
				case 1: return 75;
				case 2: return 250;
				case 3: return 750;
				default: return 750;
			}
		}

		private int GetSoluroStorageCapacity()
		{
			switch (Level)
			{
				case 1: return 75;
				case 2: return 250;
				case 3: return 750;
				default: return 750;
			}
		}

		private int GetSoluStorageCapacity()
		{
			switch (Level)
			{
				case 1: return 150;
				case 2: return 500;
				default: return 500;
			}
		}

		private int GetSoliStorageCapacity()
		{
			switch (Level)
			{
				case 1: return 100;
				case 2: return 250;
				default: return 250;
			}
		}


		public override ICollection<ColonyResource>? GetUpgradeCost()
		{
			switch (TypeStockage)
			{
				case ResourceType.Sali:
					return GetSaliStorageCost();
				case ResourceType.Solurial:
					return GetSolurialStorageCost();
				case ResourceType.Soluro:
					return GetSoluroStorageCost();
				case ResourceType.Solu:
					return GetSoluStorageCost();
				case ResourceType.Soli:
					return GetSoliStorageCost();
				default:
					return new List<ColonyResource>();
			}
		}

		private List<ColonyResource> GetSaliStorageCost()
		{
			switch (Level)
			{
				case 1:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 60),
						new ColonyResource(ResourceType.Soluro, 60),
					};
				case 2:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 175),
						new ColonyResource(ResourceType.Soluro, 175),
					};
				default:
					return new List<ColonyResource>();
			}
		}

		private List<ColonyResource> GetSolurialStorageCost()
		{
			switch (Level)
			{
				case 1:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 60),
						new ColonyResource(ResourceType.Soluro, 60),
						new ColonyResource(ResourceType.Solurial, 60),
					};
				case 2:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 175),
						new ColonyResource(ResourceType.Soluro, 175),
						new ColonyResource(ResourceType.Solurial, 175),
					};
				default:
					return new List<ColonyResource>();
			}
		}

		private List<ColonyResource> GetSoluroStorageCost()
		{
			switch (Level)
			{
				case 1:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 60),
						new ColonyResource(ResourceType.Soluro, 60),
					};
				case 2:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 175),
						new ColonyResource(ResourceType.Soluro, 175),
					};
				default:
					return new List<ColonyResource>();
			}
		}

		private List<ColonyResource> GetSoluStorageCost()
		{
			switch (Level)
			{
				case 1:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 500),
						new ColonyResource(ResourceType.Soluro, 500),
						new ColonyResource(ResourceType.Solu, 100),
					};
				default:
					return new List<ColonyResource>();
			}
		}

		private List<ColonyResource> GetSoliStorageCost()
		{
			switch (Level)
			{
				case 1:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 600),
						new ColonyResource(ResourceType.Soluro, 600),
						new ColonyResource(ResourceType.Soli, 75),
					};
				default:
					return new List<ColonyResource>();
			}
		}
	}
}