using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.Interactions;
using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models.Users;
using AnalyseProgra.UserInterfaces;

namespace AnalyseProgra.Interactions.Admin
{
    public class ManageUsersAdmin : Interaction
    {
        private readonly IUserDao _userDao;

        public ManageUsersAdmin(IUserDao userDao)
            : base("Gérer utilisateurs", "Changer rôle / activer-désactiver")
        {
            _userDao = userDao;
        }

        public override void Execute(IUserInterface ui)
        {
            var users = _userDao.GetAll().ToList();
            if (users.Count == 0) { ui.WriteError("Aucun utilisateur."); ui.Pause(); return; }

            var u = ui.Select("Choisir utilisateur", users, x => $"{x.Username} · {x.Role} · Actif={x.IsActive}");

            var action = ui.Select("Action", new[]
            {
                "Changer rôle",
                "Activer/Désactiver"
            });

            if (action == "Changer rôle")
            {
                var newRole = ui.Select("Nouveau rôle",
                    new[] { UserRole.Player, UserRole.Moderator, UserRole.Admin },
                    r => r.ToString());

                u.Role = newRole;
                _userDao.Update(u);
                ui.WriteMessage("Rôle mis à jour.");
            }
            else
            {
                u.IsActive = !u.IsActive;
                _userDao.Update(u);
                ui.WriteMessage($"Actif={u.IsActive}");
            }

            ui.Pause();
        }
    }
}
