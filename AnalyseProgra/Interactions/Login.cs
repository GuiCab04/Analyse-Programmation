using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models.Users;
using AnalyseProgra.UserInterfaces;

namespace AnalyseProgra.Interactions
{
    public class Login : Interaction
    {
        private readonly IUserDao _userDao;
        public User? SelectedUser { get; private set; }

        public Login(IUserDao userDao) : base("Login", "Créer ou sélectionner un utilisateur")
        {
            _userDao = userDao;
        }

        public override void Execute(IUserInterface ui)
        {
            var mode = ui.Select("Connexion", new[]
            {
                "Se connecter (sélectionner)",
                "Créer un utilisateur"
            });

            if (mode.StartsWith("Se connecter"))
            {
                var users = _userDao.GetAll().ToList();
                if (users.Count == 0)
                {
                    ui.WriteError("Aucun utilisateur en base. Création obligatoire.");
                    SelectedUser = CreateFlow(ui);
                    return;
                }

                var selected = ui.Select("Choisir un utilisateur", users,
                    u => $"{u.Id} · {u.Username} · {u.Role} · Actif={u.IsActive}");

                SelectedUser = _userDao.GetById(selected.Id, includeDetails: true);

                if (SelectedUser == null)
                {
                    ui.WriteError("Erreur : impossible de charger l'utilisateur sélectionné.");
                }

                return;
            }

            SelectedUser = CreateFlow(ui);
        }

        private User CreateFlow(IUserInterface ui)
        {
            string username = ui.Ask<string>("Username");
            string password = ui.Ask<string>("Password");

            var role = ui.Select("Rôle", new[] { UserRole.Player, UserRole.Moderator, UserRole.Admin }, r => r.ToString());

            // Simple: on crée un User “de base” en DB
            var user = new User(username, password, _userDao)
            {
                Role = role,
                IsActive = true
            };

            return _userDao.Create(user);
        }
    }
}
