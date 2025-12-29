namespace AnalyseProgra.UserInterfaces
{
    public interface IUserInterface
    {
        T Ask<T>(string prompt);

        public bool Confirm(string prompt);

        T Select<T>(string prompt, IEnumerable<T> choices, Func<T, string>? displaySelector = null);

        void WriteMessage(string message);
        void WriteError(string error);
        void WriteTitle(string title);

        void Pause();
        void ClearScreen();
    }
}
