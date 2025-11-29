using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public abstract class ProductionBuilding : Building
    {
        public int TauxProduction { get; protected set; }
        public ResourceTypeEnums ResourceProduite { get; protected set; }

        public ProductionBuilding(int id, string nom, ResourceTypeEnums resourceProduite)
            : base(id, nom)
        {
            ResourceProduite = resourceProduite;
            CalculateProduction();
        }

        public void Produire(ResourceManager manager)
        {
            manager.Ajouter(ResourceProduite, TauxProduction);
        }

        protected override void ApplyUpgradeEffect()
        {
            CalculateProduction();
        }

        private void CalculateProduction()
        {
            TauxProduction = Level * 5;
        }
    }
}