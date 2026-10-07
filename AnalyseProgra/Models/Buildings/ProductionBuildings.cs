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
			switch (ResourceProduite)
			{
				case ResourceType.Sali:
					TauxProduction = GetSaliProduction();
					break;
				case ResourceType.Soluro:
					TauxProduction = GetSoluroProduction();
					break;
				case ResourceType.Solu:
					TauxProduction = GetSoluProduction();
					break;
				case ResourceType.Soli:
					TauxProduction = GetSoliProduction();
					break;
				case ResourceType.Solurial:
					TauxProduction = GetSolurialProduction();
					break;
				default:
					TauxProduction = 0;
					break;
			}
		}
		private int GetSaliProduction()
		{
			switch (Level)
			{
				case 1: return 1;
				case 2: return 3;
				case 3: return 5;
				case 4: return 10;
				case 5: return 20;
				default: return Level <= 1 ? 2 : Level * 15;
			}
		}
		private int GetSoluroProduction()
		{
			switch (Level)
			{
				case 1: return 1;
				case 2: return 3;
				case 3: return 5;
				case 4: return 10;
				case 5: return 20;
				default: return Level <= 1 ? 2 : Level * 15;
			}
		}
		private int GetSoluProduction()
		{
			switch (Level)
			{
				case 1: return 1;
				case 2: return 5;
				case 3: return 10;
				case 4: return 15;
				default: return Level <= 1 ? 3 : Level * 12;
			}
		}
		private int GetSoliProduction()
		{
			switch (Level)
			{
				case 1: return 1;
				case 2: return 3;
				case 3: return 5;
				case 4: return 10;
				default: return Level <= 1 ? 4 : Level * 15;
			}
		}
		private int GetSolurialProduction()
		{
			switch (Level)
			{
				case 1: return 5;
				case 2: return 10;
				case 3: return 15;
				case 4: return 25;
				default: return Level <= 1 ? 4 : Level * 15;
			}
		}
	}
}
