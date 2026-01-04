using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.Interactions.ColonyInteractions ;

namespace AnalyseProgra.Models.Users
{
    public class Player : User
    {
        public Colony Colony;

        public Player(string name, string password, IUserDao dao)
            : base(name, password, dao)
        {
            Colony = new Colony(this, $"{name}'s Colony");
        }

        public override void AddActions()
        {
            _avalableActions.Add(new ShowBuildings(Colony));
            _avalableActions.Add(new BuyBuilding(Colony));
            _avalableActions.Add(new UpgradeBuilding(Colony));
            _avalableActions.Add(new RemoveBuilding(Colony));
            //_avalableActions.Add(new HandlePopulation(Colony));

            base.AddActions();  // Ajoute l'action Quit en dernier
        }

        public override string ToString() => $"{Username} (Joueur)";
    }
}
