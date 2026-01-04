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
			Amount = 1; // Unique
		}

		public override ICollection<ColonyResource>? GetUpgradeCost()
		{
			switch (Level)
			{
				case 2:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Soluro, 10),
						new ColonyResource(ResourceType.Sali, 10),
					};
				case 3:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 50),
						new ColonyResource(ResourceType.Soluro, 50),
						new ColonyResource(ResourceType.Solu, 20),
						new ColonyResource(ResourceType.Solurial, 40),

					};
				case 4:
					return new List<ColonyResource>
					{
						new ColonyResource(ResourceType.Sali, 150),
						new ColonyResource(ResourceType.Soluro, 150),
						new ColonyResource(ResourceType.Solu, 80),
						new ColonyResource(ResourceType.Solurial, 200),
						new ColonyResource(ResourceType.Soli, 20),

					};
				case 5:
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

		/// <summary>
		/// Nombre total de bâtiments autorisés (hors mairie elle-même) selon le niveau de la mairie.
		/// Valeurs par défaut ; modifiables si vous voulez un autre équilibrage.
		/// </summary>
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

		/// <summary>
		/// Niveau maximum autorisé pour un type de bâtiment donné selon le niveau de la mairie.
		/// Valeurs par défaut : augmente le plafond général ; si besoin on peut spécialiser par type.
		/// </summary>
		public int GetMaxLevelFor(BuildingType buildingType)
		{
			// Par défaut, même plafond pour tous les types. Ajustez si besoin.
			return Level switch
			{
				1 => 1,
				2 => 3,
				3 => 5,
				4 => 8,
				5 => 12,
				_ => 1
			};
		}
	}
}