using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public abstract class ProductionBuilding : ColonyBuildingStack
    {
        public int TauxProduction { get; protected set; }
        public ResourceTypeEnums ResourceProduite { get; protected set; }

        public ProductionBuilding(string nom, ResourceTypeEnums resourceProduite)
            : base(nom)
        {
            ResourceProduite = resourceProduite;
            CalculateProduction();
        }

        public void Produire(ResourceManager manager)
        {
            manager.Ajouter(ResourceProduite, TauxProduction);
        }

        private void CalculateProduction()
        {
            TauxProduction = Level * 5;
        }
    }
}