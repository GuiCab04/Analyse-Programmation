using AnalyseProgra.UserInterfaces;
using AnalyseProgra.Models.Users;

namespace AnalyseProgra.Interactions
{
    public class Quit : Interaction
    {
        public Quit()
            : base("Quitter", "Quitte le jeu.")
        {
        }

        public override void Execute(IUserInterface input)
        {
            var confirmation = input.Confirm("Êtes-vous sûr de vouloir quitter le jeu ? (y/n)");
            if (confirmation)
            {
                input.WriteMessage("Merci d'avoir joué ! À bientôt !");
                Environment.Exit(0);
            }      
            
        }
    }
}
