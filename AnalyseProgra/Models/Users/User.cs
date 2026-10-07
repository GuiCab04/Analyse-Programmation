using AnalyseProgra.Models.Enums;
using AnalyseProgra.Interactions;
using AnalyseProgra.DataAccess.Dao;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.Interactions.Admin;
using AnalyseProgra.Interactions.Moderator;

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
            if (Role == UserRole.Admin)
            {
                _avalableActions.Add(new ManageUsersAdmin(_dao));
            }
            else if (Role == UserRole.Moderator)
            {
                _avalableActions.Add(new AssignModerator(_dao));
            }

            _avalableActions.Add(new Save(this, _dao));
            _avalableActions.Add(new Quit());
        }

        public override string ToString() => Username;
    }
}
