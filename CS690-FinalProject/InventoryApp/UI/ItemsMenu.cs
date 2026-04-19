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
                options.Add("Search/Browse Items");
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
                case "Search/Browse Items":
                    SelectItemByListMenu();
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
            l = LocationSelectMenu(l, true, false);
        }
        Item? item = itemManager.CreateItem(name, description, value, l?.Id);
        if (item != null)
        {
            ItemActionMenu(item);
        }
    }      

    Location? LocationSelectMenu(Location? currentLocation, Boolean allowCreate, Boolean mustHaveItems)
    {
        Location? l = currentLocation;
        var locations = locationsManager.GetAllLocations();
        if (mustHaveItems)
        {
            locations = locations
                .Where(l => itemManager.LocationHasItems(l.Id))
                .ToList();
        }
        var options = locations.Select(loc => loc.Name).ToList();
        if (allowCreate)
        {
            options.Add("Create New Location");
        }
        if (l != null)
        {
            options.Add("Remove Location");
        }
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
            case "Remove Location":
                l = null;
                break;
            default:
                int index = options.IndexOf(selection);
                l = locations[index];
                break;
        }
        return l;
    }          

    void SelectItemByListMenu()
    {
        Location? l = null;
        List<Item> itemOptions = itemManager.GetAllItems();
        if (AnsiConsole.Confirm("Filter by location?"))
        {
            l = LocationSelectMenu(l, false, true);
            if (l != null)
            {
                itemOptions = itemManager.GetItemsByLocationId(l.Id);
            }
        }
        Item selectedItem = AnsiConsole.Prompt(
            new SelectionPrompt<Item>()
                .Title("=== Select an Item ===")
                .PageSize(10)
                .AddChoices(itemOptions)
                .UseConverter(item => item.Name)
                .EnableSearch()
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
                case "Delete Item":
                    Boolean deleted = DeleteItem(item);
                    if(deleted) return;
                    break;
                case "Assign / Change Location":
                    UpdateItemLocation(item);
                    break;
                default:
                    AnsiConsole.WriteLine(nyi);
                    break;
            }
        }
    }

    void UpdateItemLocation(Item item)
    {
        Location? l = null;
        if (item.LocationId is Guid LocationId)
        {
            l = locationsManager.GetLocationById(LocationId);
        }
        l = LocationSelectMenu(l, true, false);
        itemManager.UpdateItem(item.Id, item.Name, item.Description, item.EstimatedValue, l?.Id);
    }

    void PrintItemDetails(Item item)
    {
        string locationText;
        if (item.LocationId is Guid locationId)
        {
            var location = locationsManager.GetLocationById(locationId);
            locationText = location != null ? location.Name : "Unknown location";
        }
        else
        {
            locationText = "Unassigned";
        }

        AnsiConsole.MarkupLine($"[bold]Name: {item.Name}[/]");
        AnsiConsole.MarkupLine($"[bold]Description: [/] {item.Description}");
        AnsiConsole.MarkupLine($"[bold]Location: [/] {locationText}");
        AnsiConsole.MarkupLine($"[bold]Value: [/] ${item.EstimatedValue:C}");
        AnsiConsole.MarkupLine($"[bold]Loan Status: [/] [red]{nyi}[/]");
        AnsiConsole.WriteLine("\nAny key to return");
        Console.ReadKey(true);
    }

    Boolean DeleteItem(Item item)
    {
        if(AnsiConsole.Confirm($"Are you sure you want to delete {item.Name}")) 
        {
            return itemManager.DeleteItem(item.Id);
        }
        return false;
    }

}
