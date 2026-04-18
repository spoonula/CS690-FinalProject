namespace InventoryApp;

using Spectre.Console;

class App
{

    const String nyi = "Not yet implemented";

    public void Run()
    {
        MainMenu();
    }

    void MainMenu()
    {
        while (true)
        {
            AnsiConsole.Clear();
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("=== Main Menu ===")
                    .AddChoices("Items", "Locations", "Borrowers", "Reports", "Exit")
            );

            switch (selection)
            {
                case "Exit":
                    return;
                case "Items":
                    ItemsMenu();
                    break;           
                default:
                    AnsiConsole.WriteLine(nyi);
                    break;
            }
        }
    }

    void ItemsMenu()
    {
        while (true)
        {
            AnsiConsole.Clear();
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("=== Items ===")
                    .AddChoices("Create Item", "Select Item", "Back")
            );

            switch (selection)
            {
                case "Back":
                    return;
                case "Create Item":
                    CreateItemMenu();
                    break;
                // case "Select Item":
                //     break;
                default:
                    AnsiConsole.WriteLine(nyi);
                    break;
            }
        }
    }

    void CreateItemMenu()
    {

        AnsiConsole.Clear();
        AnsiConsole.WriteLine("=== Create Item ===\n");
        
        // Get Item inputs
        String name = PromptNotEmpty("Enter item name:");
        String description = PromptNotEmpty("Enter item description:");
        decimal value = PromptDecimal("Enter estimated value:", false);
        AnsiConsole.WriteLine(nyi);
        AnsiConsole.WriteLine("");
        if (AnsiConsole.Confirm("Assign Location?"))
        {
            AnsiConsole.WriteLine(nyi);
        }
    }

    // validation helpers
    private String PromptNotEmpty(String prompt)
    {
        String response = "";
        do {
            response = AnsiConsole.Ask<string>(prompt);
            if (string.IsNullOrWhiteSpace(response))
            {
                AnsiConsole.MarkupLine("[red]Your response must not be empty[/]");
            }
        } while (string.IsNullOrWhiteSpace(response));
        return response;
    }

    private decimal PromptDecimal(String prompt, Boolean allowNegative)
    {
        decimal value;
        while (true)
        {
            String input = AnsiConsole.Ask<string>(prompt);

            if (string.IsNullOrWhiteSpace(input))
            {
                AnsiConsole.MarkupLine("[red]Your response must not be empty[/]");
                continue;
            }

            // remove curreny formatting
            String cleaned = input.Replace("$", "").Replace(",", "").Trim();

            if (!decimal.TryParse(cleaned, out value))
            {
                AnsiConsole.MarkupLine("[red]Please enter a valid number[/]");
                continue;
            }

            if (!allowNegative && value < 0)
            {
                AnsiConsole.MarkupLine("[red]Value must not be negative[/]");
                continue;
            }
            return value;
        }
    }

}
