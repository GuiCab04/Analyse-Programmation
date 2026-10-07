using System.Collections.Generic;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public class Mine : ProductionBuilding
    {
		public Mine(int level, ResourceType typeMinerai)
			: base(typeMinerai switch
			{
				ResourceType.Soluro => BuildingType.SoluroMine,
				ResourceType.Sali => BuildingType.SaliFarm,
				ResourceType.Solu => BuildingType.SoluMine,
				ResourceType.Soli => BuildingType.SoliMine,
				_ => BuildingType.SoluroMine
			},
				  level,
				  typeMinerai)
		{
		}

		public override ICollection<ColonyResource>? GetUpgradeCost()
        {
            switch (ResourceProduite)  
                {
                case ResourceType.Soluro:
                    return GetSoluroMineCost();
                case ResourceType.Sali:
                    return GetSaliMineCost();
				case ResourceType.Solu:
					return GetSoluMineCost();
				case ResourceType.Soli:
					return GetSoliMineCost();
				default:
                    return null;
			}
        }

        private List<ColonyResource> GetSaliMineCost()
        {
            switch (Level)
            {
                case 1:
                    return new List<ColonyResource>
                    {
                        new ColonyResource(ResourceType.Soluro, 15),
                        new ColonyResource(ResourceType.Sali, 15),
					};
                case 2:
                    return new List<ColonyResource>
                    {
                        new ColonyResource(ResourceType.Sali, 50),
                        new ColonyResource(ResourceType.Soluro, 50),
                        new ColonyResource(ResourceType.Solu, 15),
					};
                case 3:
                    return new List<ColonyResource>
                    {
                        new ColonyResource(ResourceType.Sali, 150),
                        new ColonyResource(ResourceType.Soluro, 150),
                        new ColonyResource(ResourceType.Solu, 80),
                        new ColonyResource(ResourceType.Soli, 15),
					};
                case 4:
                    return new List<ColonyResource>
                    {
                        new ColonyResource(ResourceType.Sali, 400),
                        new ColonyResource(ResourceType.Soluro, 400),
                        new ColonyResource(ResourceType.Solu, 300),
                        new ColonyResource(ResourceType.Soli, 150),
					};
                default:
                    return new List<ColonyResource>();
			}
		}
		private List<ColonyResource> GetSoluroMineCost()
		{
			switch (Level)
			{
				case 1:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Soluro, 15),
						new ColonyResource(ResourceType.Sali, 15),
					};
				case 2:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 50),
						new ColonyResource(ResourceType.Soluro, 50),
						new ColonyResource(ResourceType.Solu, 15),
					};
				case 3:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 150),
						new ColonyResource(ResourceType.Soluro, 150),
						new ColonyResource(ResourceType.Solu, 80),
						new ColonyResource(ResourceType.Soli, 15),
					};
				case 4:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 400),
						new ColonyResource(ResourceType.Soluro, 400),
						new ColonyResource(ResourceType.Solu, 300),
						new ColonyResource(ResourceType.Soli, 150),
					};
				default:
					return new List<ColonyResource>();
			}
		}
		private List<ColonyResource> GetSoluMineCost()
		{
			switch (Level)
			{
				case 1:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 40),
						new ColonyResource(ResourceType.Soluro, 40),
						new ColonyResource(ResourceType.Solu, 10),
					};
				case 2:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 100),
						new ColonyResource(ResourceType.Soluro, 100),
						new ColonyResource(ResourceType.Solu, 60),
						new ColonyResource(ResourceType.Soli, 10),
					};
				case 3:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 300),
						new ColonyResource(ResourceType.Soluro, 300),
						new ColonyResource(ResourceType.Solu, 120),
						new ColonyResource(ResourceType.Soli, 60),
					};
				default:
					return new List<ColonyResource>();
			}
		}
		private List<ColonyResource> GetSoliMineCost()
		{
			switch (Level)
			{
				case 1:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 110),
						new ColonyResource(ResourceType.Soluro, 110),
						new ColonyResource(ResourceType.Solu, 50),
						new ColonyResource(ResourceType.Soli, 10),
					};
				case 2:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 250),
						new ColonyResource(ResourceType.Soluro, 250),
						new ColonyResource(ResourceType.Solu, 100),
						new ColonyResource(ResourceType.Soli, 40),
					};
				case 3:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 450),
						new ColonyResource(ResourceType.Soluro, 450),
						new ColonyResource(ResourceType.Solu, 150),
						new ColonyResource(ResourceType.Soli, 75),
					};
				default:
					return new List<ColonyResource>();
			}
		}
	}
}