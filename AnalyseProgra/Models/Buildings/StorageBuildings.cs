using System.Collections.Generic;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public class StorageBuilding : ColonyBuildingStack
    {
        public int CapaciteAjoutee { get; private set; }

        // Si null = Stocke TOUT. Si renseigné = Stocke uniquement cette ressource (Ex: Silo à blé)
        public ResourceTypeEnums? TypeStockage { get; private set; }

        public StorageBuilding(ResourceTypeEnums? typeSpecific = null)
            : base(typeSpecific == null ? "Entrepôt Général" : $"Silo à {typeSpecific}")
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
                new ColonyResource(ResourceTypeEnums.Fer, Level * 100),
            };
        }
    }
}