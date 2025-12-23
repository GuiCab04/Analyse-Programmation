using AnalyseProgra.Models.Users;
using AnalyseProgra.UserInterfaces;

namespace AnalyseProgra.Controllers
{
    public class UpgradeBuilding : Interaction
    {
        private Player _player;

        public UpgradeBuilding(Player player) :
            base("Améliorer un bâtiment", "Permet d'améliorer un bâtiment existant.")
        {
            _player = player;
        }

        public override void Execute(IUserInterface input)
        {
            input.WriteTitle($"Amélioration - {_player.Name}");

        }
    }
}
