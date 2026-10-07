using System.Collections.Generic;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
	/// <summary>
	/// Bâtiment unique "Mairie" qui gouverne :
	/// - le nombre total de bâtiments permis dans la colonie,
	/// - le niveau maximal autorisé pour les autres bâtiments.
	/// </summary>
	public class Mairie : ColonyBuildingStack
	{
		public Mairie(int level)
			: base(BuildingType.Mairie, level)
		{
			Amount = 1;
		}

		public override ICollection<ColonyResource>? GetUpgradeCost()
		{
			switch (Level)
			{
				case 1:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Soluro, 10),
						new ColonyResource(ResourceType.Sali, 10),
					};
				case 2:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 50),
						new ColonyResource(ResourceType.Soluro, 50),
						new ColonyResource(ResourceType.Solu, 20),
						new ColonyResource(ResourceType.Solurial, 40),

					};
				case 3:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 150),
						new ColonyResource(ResourceType.Soluro, 150),
						new ColonyResource(ResourceType.Solu, 80),
						new ColonyResource(ResourceType.Solurial, 200),
						new ColonyResource(ResourceType.Soli, 20),

					};
				case 4:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 650),
						new ColonyResource(ResourceType.Soluro, 650),
						new ColonyResource(ResourceType.Solu, 350),
						new ColonyResource(ResourceType.Solurial, 500),
						new ColonyResource(ResourceType.Soli, 200),
					};
				default:
					return new List<ColonyResource>();
			}
		}

		public int GetMaxBuildingsAllowed()
		{
			return Level switch
			{
				1 => 5,
				2 => 12,
				3 => 25,
				4 => 40,
				5 => 80,
				_ => 5
			};
		}
	}
}