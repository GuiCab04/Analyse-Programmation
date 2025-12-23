using AnalyseProgra.Controllers;
using AnalyseProgra.Models.Buildings;

namespace AnalyseProgra.Models.Users
{
    public class Player : User
    {
        private List<Building> _buildings = new List<Building>();
        public List<Building> Buildings => _buildings;

        public ResourceManager Resource { get; private set; }
        public PopulationManager Population { get; private set; }

        public Player(string name) : base(name)
        {
            Resource = new ResourceManager();
            Population = new PopulationManager();

            _avalableActions.Add(new BuyBuilding(this));
            _avalableActions.Add(new Quit());
        }

        public void AddBuilding(Building building)
        {
            _buildings.Add(building);
        }
    }
}
