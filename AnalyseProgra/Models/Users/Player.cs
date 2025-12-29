using AnalyseProgra.Controllers;
using AnalyseProgra.Models.Buildings;

namespace AnalyseProgra.Models.Users
{
    public class Player : User
    {
        public Player(string name, string password) : base(name, password)
        {
        }

        public void AddBuilding()
        {
        }
    }
}
