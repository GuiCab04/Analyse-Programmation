using AnalyseProgra.DataAccess.Dao;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.Models.Users;

namespace AnalyseProgra.Interactions
{
    internal class Save : Interaction
    {
        private User _user;
        private IUserDao _userDao;

        public Save(User user, IUserDao userDao)
            : base("Save", "Saves the current user's data to the database.")
        {
            _user = user;
            _userDao = userDao;
        }

        public override void Execute(UserInterfaces.IUserInterface input)
        {
            if (input.Confirm("Voulez-vous vraiment sauvegarder ?"))
            {
                try
                {
                    _userDao.SaveWithRelations(_user);
                    input.WriteMessage("Données sauvegardées avec succès.");
                }
                catch (Exception ex)
                {
                    input.WriteMessage($"Erreur lors de la sauvegarde des données : {ex.Message}");
                }
                finally
                {
                    input.Pause();
                }
            }            
        }
    }
}
