using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public abstract class ProductionBuilding : ColonyBuildingStack
    {
        public int TauxProduction { get; protected set; }
        public ResourceType ResourceProduite { get; protected set; }

        public ProductionBuilding(BuildingType type, int level, ResourceType resourceProduite)
            : base(type, level)
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
