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
                //options.Add("Update Location");
                //options.Add("Delete Location");
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

    public void CreateLocationMenu()
    {

        AnsiConsole.Clear();
        AnsiConsole.WriteLine("=== Create Location  ===\n");
        
        // Get Item inputs
        String name = LocationNamePrompt("Enter location name:");

        Location? location = locationsManager.CreateLocation(name);
        if (location == null)
        {
            AnsiConsole.MarkupLine("[red]Location could not create[/]");
        }

    }                


    Location SelectLocationByListMenu()
    {
        Location selectedLocation = AnsiConsole.Prompt(
            new SelectionPrompt<Location>()
                .Title("=== Select an Location ===")
                .PageSize(10)
                .UseConverter(location => location.Name)
                .AddChoices(locationsManager.GetAllLocations())
                .EnableSearch()
        );
        return selectedLocation;
    }

    // helpers
    private string LocationNamePrompt(String? defaultValue = null)
    {
        string name;
        while(true)
        {
            name = defaultValue is not null
            ? PromptNotEmpty("Enter location name:", defaultValue)
            : PromptNotEmpty("Enter location name:");
            if (locationsManager.LocationNameExists(name))
            {
                AnsiConsole.MarkupLine($"[red]{name} is already a location[/]");
                continue;
            }
            return name;
        }
    }

}
