namespace InventoryApp;

using Spectre.Console;
using InventoryApp.Services;
using InventoryApp.Models;
using static InventoryApp.UI.PromptHelpers;

class LocationsMenu
{

    const String nyi = "Not yet implemented";
    private LocationsManager locationsManager;

    public LocationsMenu(LocationsManager locationsManager) 
    {
        this.locationsManager = locationsManager;
    }

    public void Show()
    {
        while (true)
        {
            Location l;
            var options = new List<string>();
            options.Add("Create Location");
            if (locationsManager.GetAllLocations().Count > 0)
            {
                options.Add("Update Location");
                options.Add("Delete Location");
            }
            options.Add("Back");
            AnsiConsole.Clear();
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("=== Locations ===")
                    .AddChoices(options)
            );

            switch (selection)
            {
                case "Back":
                    return;
                case "Create Location":
                    CreateLocationMenu();
                    break;
                case "Update Location":
                    l = SelectLocationByListMenu();
                    break;
                case "Delete Location":
                    l = SelectLocationByListMenu();
                    break;
                default:
                    AnsiConsole.WriteLine(nyi);
                    break;
            }
        }
    }

    public Location CreateLocationMenu()
    {

        AnsiConsole.Clear();
        AnsiConsole.WriteLine("=== Create Location  ===\n");
        
        // Get Item inputs
        String name = PromptNotEmpty("Enter location name:");

        return locationsManager.CreateLocation(name);

    }                

    // void SelectItemMenu()
    // {
    //     while (true)
    //     {
    //         AnsiConsole.Clear();
    //         var selection = AnsiConsole.Prompt(
    //             new SelectionPrompt<string>()
    //                 .Title("=== Select Item ===")
    //                 .AddChoices("Search by name", "Browse all items", "Back")
    //         );

    //         switch (selection)
    //         {
    //             case "Back":
    //                 return;
    //             case "Search by name":
    //                 AnsiConsole.WriteLine(nyi);
    //                 break;
    //             case "Browse all items":
    //                 SelectItemByListMenu();
    //                 break;
    //             default:
    //                 AnsiConsole.WriteLine(nyi);
    //                 break;
    //         }
    //     }
    // }

    Location SelectLocationByListMenu()
    {
        Location selectedLocation = AnsiConsole.Prompt(
            new SelectionPrompt<Location>()
                .Title("=== Select an Location ===")
                .PageSize(10)
                .UseConverter(location => location.Name)
                .AddChoices(locationsManager.GetAllLocations())
        );
        return selectedLocation;
    }

    // void ItemActionMenu(Item item)
    // {
    //     while (true)
    //     {
    //         AnsiConsole.Clear();
    //         var selection = AnsiConsole.Prompt(
    //             new SelectionPrompt<string>()
    //                 .Title($"=== {item.Name} ===")
    //                 .AddChoices("View Details", "Update Item", "Delete Item", "Assign / Change Location", 
    //                 "Mark as Loaned", "Mark as Returned", "Back")
    //         );

    //         switch (selection)
    //         {
    //             case "Back":
    //                 return;
    //             case "View Details":
    //                 PrintItemDetails(item);
    //                 break;
    //             default:
    //                 AnsiConsole.WriteLine(nyi);
    //                 break;
    //         }
    //     }
    // }

    // void PrintItemDetails(Item item)
    // {
    //     AnsiConsole.MarkupLine($"[bold]Name: {item.Name}[/]");
    //     AnsiConsole.MarkupLine($"[bold]Description: [/] {item.Description}");
    //     AnsiConsole.MarkupLine($"[bold]Location: [/] {nyi}");
    //     AnsiConsole.MarkupLine($"[bold]Value: [/] ${item.EstimatedValue:C}");
    //     AnsiConsole.MarkupLine($"[bold]Loan Status: [/] [red]{nyi}[/]");
    //     AnsiConsole.WriteLine("\nAny key to return");
    //     Console.ReadKey(true);
    // }

}
