using System.Collections.Generic;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public class StorageBuilding : Building
    {
        public int CapaciteAjoutee { get; private set; }

        // Si null = Stocke TOUT. Si renseigné = Stocke uniquement cette ressource (Ex: Silo à blé)
        public ResourceTypeEnums? TypeStockage { get; private set; }

        public StorageBuilding(int id, ResourceTypeEnums? typeSpecific = null)
            : base(id, typeSpecific == null ? "Entrepôt Général" : $"Silo à {typeSpecific}")
        {
            TypeStockage = typeSpecific;
            CalculateCapacity();
        }

        protected override void ApplyUpgradeEffect()
        {
            CalculateCapacity();
        }

        private void CalculateCapacity()
        {
            // Un entrepôt ajoute +200 de capacité par niveau
            CapaciteAjoutee = Level * 200;
        }

        public override Dictionary<ResourceTypeEnums, int> GetUpgradeCost()
        {
            return new Dictionary<ResourceTypeEnums, int>
            {
                { ResourceTypeEnums.Fer, Level * 100 }
            };
        }
    }
}