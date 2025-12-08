using AnalyseProgra.Controllers;

namespace AnalyseProgra.Models.Users
{
    public abstract class User
    {
        protected List<Interaction> _avalableActions = new List<Interaction>();
        public List<Interaction> AvalableActions => _avalableActions;
        public string Name { get; }

        public User(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}
