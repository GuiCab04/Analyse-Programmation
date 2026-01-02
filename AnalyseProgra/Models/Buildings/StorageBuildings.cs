using System.Collections.Generic;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public class StorageBuilding : ColonyBuildingStack
    {
        public int CapaciteAjoutee { get; private set; }

        // Si null = Stocke TOUT. Si renseigné = Stocke uniquement cette ressource (Ex: Silo à blé)
        public ResourceType? TypeStockage { get; private set; }

        public StorageBuilding(int level, ResourceType? typeSpecific = null)
            : base(BuildingType.StorageBuilding, level)
        {
            TypeStockage = typeSpecific;
            CalculateCapacity();
        }

        private void CalculateCapacity()
        {
            // Un entrepôt ajoute +200 de capacité par niveau
            CapaciteAjoutee = Level * 200;
        }

        public override ICollection<ColonyResource>? GetUpgradeCost()
        {
            return new List<ColonyResource>
            {
                new ColonyResource(ResourceType.Fer, Level * 100),
            };
        }
    }
}