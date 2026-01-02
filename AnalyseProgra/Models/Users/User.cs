using System.Collections.Generic;
using AnalyseProgra.Models.Enums;
using AnalyseProgra.Interactions;

namespace AnalyseProgra.Models
{
    public class User
    {
        // Database fields
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }

        // Additional properties
        protected List<Interaction> _avalableActions = new List<Interaction>();
        public List<Interaction> AvalableActions => _avalableActions;

        public ICollection<Colony>? Colonies { get; set; }

        public User(string username, string password)
        {
            Username = username;
            PasswordHash = password; // À hasher plus tard
        }

        public virtual void AddActions()
        {
            _avalableActions.Add(new Quit());   // Pas top car crée une nouvelle instance à chaque fois
        }

        public override string ToString() => Username;
    }
}
