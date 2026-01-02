using Spectre.Console;
using AnalyseProgra.UserInterfaces;

namespace AnalyseProgra.Views
{
    public abstract class SpectreInterface : IUserInterface
    {
        public SpectreInterface()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }

        public T Ask<T>(string prompt)
        {
            return AnsiConsole.Ask<T>($"[bold yellow]{prompt}[/]:");
        }
        public bool Confirm(string prompt)
        {
            return AnsiConsole.Confirm($"[yellow]{prompt}[/]");
        }

        public T Select<T>(string prompt, IEnumerable<T> choices, Func<T, string>? displaySelector = null)
        {
            var selector = new SelectionPrompt<T>()
                .Title($"[green]{prompt}[/]")
                .PageSize(10)
                .AddChoices(choices);

            if (displaySelector != null)
            {
                selector.UseConverter(displaySelector);
            }

            return AnsiConsole.Prompt(selector);
        }

        public void DisplayTable(string title, IEnumerable<string> headers, IEnumerable<IEnumerable<string>> rows)
        {
            var table = new Table().Title($"[blue]{title}[/]");

            foreach (var header in headers)
            {
                table.AddColumn(header);
            }

            foreach (var row in rows)
            {
                table.AddRow(row.ToArray());
            }

            AnsiConsole.Write(table);
        }

        public void WriteMessage(string message)
        {
            AnsiConsole.MarkupLine($"[cyan]{message}[/]");
        }

        public void WriteError(string error)
        {
            AnsiConsole.MarkupLine($"[red bold]Erreur : {error}[/]");
        }

        public void WriteTitle(string title)
        {
            AnsiConsole.Write(new Rule($"[red]{title}[/]"));
        }

        public void Load(int duration)
        {
            AnsiConsole.Status().Start("Construction...", ctx => {
                ctx.Spinner(Spinner.Known.Clock); Thread.Sleep(duration);
            });
        }

        public void Pause()
        {
            AnsiConsole.MarkupLine("[grey]Appuyez sur une touche...[/]"); Console.ReadKey(true);
        }

        public void ClearScreen()
        {
            AnsiConsole.Clear();
        }
    }
}
