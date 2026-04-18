namespace InventoryApp;

using Spectre.Console;

class App
{
    public void Run()
    {
        AnsiConsole.MarkupLine("[bold]Hello, World![/]");
    }

    void MainMenu()
    {
        var domain = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("=== Main Menu ===")
                .AddChoices("Items", "Locations", "Borrowers", "Reports", "Exit")
        );

        AnsiConsole.WriteLine(domain)
    }
}
