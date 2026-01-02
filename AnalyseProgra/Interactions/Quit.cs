using AnalyseProgra.UserInterfaces;
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
            input.WriteMessage("Merci d'avoir joué ! À bientôt !");
            Environment.Exit(0);
        }
    }
}
