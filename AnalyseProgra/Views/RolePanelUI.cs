using AnalyseProgra.Interactions;
using AnalyseProgra.Models.Users;

namespace AnalyseProgra.Views
{
    public class RolePanelUI : SpectreInterface
    {
        private readonly User _user;

        public RolePanelUI(User user) : base()
        {
            _user = user;
        }

        public Interaction ShowMenu()
        {
            WriteTitle($"{_user.Role.ToString().ToUpper()} · {_user.Username}");
            return Select("Actions", _user.AvalableActions, a => a.Name);
        }
    }
}
