using AnalyseProgra.Interactions.ColonyInteractions ;

namespace AnalyseProgra.Models.Users
{
    public class Player : User
    {
        public Colony Colony;

        public Player(string name, string password)
            : base(name, password)
        {
            Colony = Colonies != null && Colonies.Count > 0 ? Colonies.First() : new Colony(this, $"Colonie de {name}");
        }

        public override void AddActions()
        {
            _avalableActions.Add(new ShowBuildings(Colony));
            _avalableActions.Add(new BuyBuilding(Colony));
            _avalableActions.Add(new UpgradeBuilding(Colony));
            _avalableActions.Add(new HandlePopulation(Colony));

            base.AddActions();  // Ajoute l'action Quit en dernier
        }
    }
}
