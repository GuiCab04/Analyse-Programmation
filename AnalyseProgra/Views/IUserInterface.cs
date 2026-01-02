namespace AnalyseProgra.UserInterfaces
{
    public interface IUserInterface
    {
        public T Ask<T>(string prompt);

        public bool Confirm(string prompt);

        public T Select<T>(string prompt, IEnumerable<T> choices, Func<T, string>? displaySelector = null);

        public void DisplayTable(string title, IEnumerable<string> headers, IEnumerable<IEnumerable<string>> rows);

        public void WriteMessage(string message);
        public void WriteError(string error);
        public void WriteTitle(string title);

        public void Load(int duration);
        public void Pause();
        public void ClearScreen();
    }
}
