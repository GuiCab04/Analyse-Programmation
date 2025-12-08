using AnalyseProgra.Controllers;
using AnalyseProgra.Models.Buildings;

namespace AnalyseProgra.Models.Users
{
    public class Player : User
    {
        private List<Building> _buildings = new List<Building>();
        public List<Building> Buildings => _buildings;
        public int Population { get; set; }
        public Dictionary<string, int> Resources { get; set; }

        public Player(string name, int population) : base(name)
        {
            Population = population;

            _avalableActions.Add(new BuyBuilding(this));
            _avalableActions.Add(new Quit());
        }

        public void AddBuilding(Building building)
        {
            _buildings.Add(building);
        }
    }
}
