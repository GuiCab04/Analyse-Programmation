using AnalyseProgra.Core.Managers;
using AnalyseProgra.Models.Users;

namespace AnalyseProgra.Models
{
    public class Colony
    {
        public int Id { get; set; }
        public string OwnerUsername { get; set; } = "";
        public string Name { get; set; } = "";
        public double Morale { get; set; }

        public User? Owner { get; set; }
        public ResourceManager Resources { get; set; }
        public PopulationManager Population { get; set; }
        public BuildingManager Buildings { get; set; }

        public Colony()
        {
            Resources = new ResourceManager();
            Population = new PopulationManager();
            Buildings = new BuildingManager();
        }

        public Colony(User owner, string name) : this()
        {
            Owner = owner;
            OwnerUsername = owner.Username;
            Name = name;
            Morale = 100.0;
        }
    }
}
