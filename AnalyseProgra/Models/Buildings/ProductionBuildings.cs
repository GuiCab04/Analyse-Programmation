using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public abstract class ProductionBuilding : ColonyBuildingStack
    {
        public int TauxProduction { get; protected set; }
        public ResourceTypeEnums ResourceProduite { get; protected set; }

        public ProductionBuilding(string nom, BuildingType type, int level, ResourceTypeEnums resourceProduite)
            : base(nom, type, level)
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