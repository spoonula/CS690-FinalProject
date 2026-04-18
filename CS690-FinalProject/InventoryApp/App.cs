namespace InventoryApp;

using Spectre.Console;
using InventoryApp.Services;
using InventoryApp.Models;

class App
{

    const String nyi = "Not yet implemented";
    private ItemManager itemManager = new ItemManager();

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
            var options = new List<string>();
            options.Add("Create Item");
            if (itemManager.GetAllItems().Count > 0)
            {
                options.Add("Select Item");
            }
            options.Add("Back");
            AnsiConsole.Clear();
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("=== Items ===")
                    .AddChoices(options)
            );

            switch (selection)
            {
                case "Back":
                    return;
                case "Create Item":
                    CreateItemMenu();
                    break;
                case "Select Item":
                    SelectItemMenu();
                    break;
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
        AnsiConsole.WriteLine("");
        if (AnsiConsole.Confirm("Assign Location?"))
        {
            AnsiConsole.WriteLine(nyi);
        }
        itemManager.CreateItem(name, description, value);
    }                

    void SelectItemMenu()
    {
        while (true)
        {
            AnsiConsole.Clear();
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("=== Select Item ===")
                    .AddChoices("Search by name", "Browse all items", "Back")
            );

            switch (selection)
            {
                case "Back":
                    return;
                case "Search by name":
                    AnsiConsole.WriteLine(nyi);
                    break;
                case "Browse all items":
                    SelectItemByListMenu();
                    break;
                default:
                    AnsiConsole.WriteLine(nyi);
                    break;
            }
        }
    }

    void SelectItemByListMenu()
    {
        if (itemManager.GetAllItems().Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No items available[/]");
            AnsiConsole.WriteLine("Any key to continue");
            Console.ReadKey(true);
            return;
        }
        Item selectedItem = AnsiConsole.Prompt(
            new SelectionPrompt<Item>()
                .Title("=== Select an Item ===")
                .PageSize(10)
                .UseConverter(item => item.Name)
                .AddChoices(itemManager.GetAllItems())
        );
        ItemActionMenu(selectedItem);
    }

    void ItemActionMenu(Item item)
    {
        while (true)
        {
            AnsiConsole.Clear();
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("=== Select Item ===")
                    .AddChoices("View Details", "Update Item", "Delete Item", "Assign / Change Location", 
                    "Mark as Loaned", "Mark as Returned", "Back")
            );

            switch (selection)
            {
                case "Back":
                    return;
                case "View Details":
                    PrintItemDetails(item);
                    break;
                default:
                    AnsiConsole.WriteLine(nyi);
                    break;
            }
        }
    }

    void PrintItemDetails(Item item)
    {
        AnsiConsole.MarkupLine($"[bold]Name: {item.Name}[/]");
        AnsiConsole.MarkupLine($"[bold]Description: [/] {item.Description}");
        AnsiConsole.MarkupLine($"[bold]Location: [/] {nyi}");
        AnsiConsole.MarkupLine($"[bold]Value: [/] ${item.EstimatedValue:C}");
        AnsiConsole.MarkupLine($"[bold]Loan Status: [/] [red]{nyi}[/]");
        AnsiConsole.WriteLine("\nAny key to return");
        Console.ReadKey(true);
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
