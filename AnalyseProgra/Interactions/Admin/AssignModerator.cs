using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.Interactions;
using AnalyseProgra.Models.Enums;
using AnalyseProgra.UserInterfaces;

namespace AnalyseProgra.Interactions.Moderator
{
    public class AssignModerator : Interaction
    {
        private readonly IUserDao _userDao;

        public AssignModerator(IUserDao userDao)
            : base("Attribuer modérateur", "Passe un joueur en modérateur")
        {
            _userDao = userDao;
        }

        public override void Execute(IUserInterface ui)
        {
            var users = _userDao.GetAll().Where(u => u.Role == UserRole.Player).ToList();
            if (users.Count == 0) { ui.WriteError("Aucun joueur à promouvoir."); ui.Pause(); return; }

            var target = ui.Select("Choisir un joueur", users, u => $"{u.Username} · {u.Role}");

            if (!ui.Confirm($"Promouvoir {target.Username} en modérateur ?")) return;

            target.Role = UserRole.Moderator;
            _userDao.Update(target);

            ui.WriteMessage("Promotion faite.");
            ui.Pause();
        }
    }
}
