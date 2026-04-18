namespace InventoryApp;

using Spectre.Console;
using InventoryApp.Services;
using InventoryApp.Models;
using static InventoryApp.UI.PromptHelpers;

class ItemsMenu
{

    const String nyi = "Not yet implemented";
    private ItemManager itemManager;
    private LocationsManager locationsManager;

    public ItemsMenu(ItemManager itemManager, LocationsManager locationsManager) 
    {
        this.itemManager = itemManager;
        this.locationsManager = locationsManager;
    }

    public void Show()
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
        Location? l = null;
        // Get Item inputs
        String name = PromptNotEmpty("Enter item name:");
        String description = PromptNotEmpty("Enter item description:");
        decimal value = PromptDecimal("Enter estimated value:", false);
        AnsiConsole.WriteLine("");
        if (AnsiConsole.Confirm("Assign Location?"))
        {
            l = LocationSelectMenu();
        }
        Item? item = itemManager.CreateItem(name, description, value, l?.Id);
        if (item != null)
        {
            ItemActionMenu(item);
        }
    }      

    Location? LocationSelectMenu()
    {
        Location? l = null;
        var locations = locationsManager.GetAllLocations();
        var options = locations.Select(loc => loc.Name).ToList();
        options.Add("Create New Location");
        options.Add("Back");
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("=== Select a location === ")
            .AddChoices(options)
        );

        switch (selection)
        {
            case "Create New Location":
                    LocationsMenu locationsMenu = new LocationsMenu(locationsManager);
                    l = locationsMenu.CreateLocationMenu();
                break;
            case "Back":
                break;
            default:
                int index = options.IndexOf(selection);
                l = locations[index];
                break;
        }
        return l;
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
                    .Title($"=== {item.Name} ===")
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
        string locationText;
        if (item.LocationId == null)
        {
            locationText = "Unassigned";
        }
        else
        {
            var location = locationsManager.GetLocationById(item.LocationId.Value);
            if (location != null)
            {
                locationText = location.Name;
            }
            else
            {
                locationText = "Unknown location";
            }

        }

        AnsiConsole.MarkupLine($"[bold]Name: {item.Name}[/]");
        AnsiConsole.MarkupLine($"[bold]Description: [/] {item.Description}");
        AnsiConsole.MarkupLine($"[bold]Location: [/] {locationText}");
        AnsiConsole.MarkupLine($"[bold]Value: [/] ${item.EstimatedValue:C}");
        AnsiConsole.MarkupLine($"[bold]Loan Status: [/] [red]{nyi}[/]");
        AnsiConsole.WriteLine("\nAny key to return");
        Console.ReadKey(true);
    }

}
