using AnalyseProgra.Models.Enums;
using AnalyseProgra.Interactions;
using AnalyseProgra.DataAccess.Dao;
using AnalyseProgra.DataAccess.Interface;

namespace AnalyseProgra.Models.Users
{
    public class User
    {
        // Database fields
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }

        private IUserDao _dao;

        // Additional properties
        protected List<Interaction> _avalableActions = new List<Interaction>();
        public List<Interaction> AvalableActions => _avalableActions;

        public User(string username, string password, IUserDao dao)
        {
            Username = username;
            PasswordHash = password; // À hasher plus tard
            _dao = dao;
        }

        public virtual void AddActions()
        {
            _avalableActions.Add(new Save(this, _dao));
            _avalableActions.Add(new Quit());
        }

        public override string ToString() => Username;
    }
}
