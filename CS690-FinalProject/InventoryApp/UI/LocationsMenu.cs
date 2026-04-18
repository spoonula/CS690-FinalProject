namespace InventoryApp;

using Spectre.Console;
using InventoryApp.Services;
using InventoryApp.Models;
using static InventoryApp.UI.PromptHelpers;

class LocationsMenu
{

    const String nyi = "Not yet implemented";
    private LocationsManager locationManager;

    public LocationsMenu(LocationsManager locationManager) 
    {
        this.locationManager = locationManager;
    }

    public void Show()
    {
        while (true)
        {
            var options = new List<string>();
            options.Add("Create Location");
            if (locationManager.GetAllLocations().Count > 0)
            {
                options.Add("Select Location");
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
                // case "Select Item":
                //     SelectItemMenu();
                //     break;
                default:
                    AnsiConsole.WriteLine(nyi);
                    break;
            }
        }
    }

    void CreateLocationMenu()
    {

        AnsiConsole.Clear();
        AnsiConsole.WriteLine("=== Create Location  ===\n");
        
        // Get Item inputs
        String name = PromptNotEmpty("Enter location name:");

        locationManager.CreateLocation(name);
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

    // void SelectItemByListMenu()
    // {
    //     if (locationManager.GetAllItems().Count == 0)
    //     {
    //         AnsiConsole.MarkupLine("[red]No items available[/]");
    //         AnsiConsole.WriteLine("Any key to continue");
    //         Console.ReadKey(true);
    //         return;
    //     }
    //     Item selectedItem = AnsiConsole.Prompt(
    //         new SelectionPrompt<Item>()
    //             .Title("=== Select an Item ===")
    //             .PageSize(10)
    //             .UseConverter(item => item.Name)
    //             .AddChoices(locationManager.GetAllItems())
    //     );
    //     ItemActionMenu(selectedItem);
    // }

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
