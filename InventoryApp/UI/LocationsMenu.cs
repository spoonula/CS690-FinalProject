namespace InventoryApp;

using Spectre.Console;
using InventoryApp.Services;
using InventoryApp.Models;
using static InventoryApp.UI.PromptHelpers;

class LocationsMenu
{

    const String nyi = "Not yet implemented";
    private LocationsManager locationsManager;

    private ItemManager itemManager;

    public LocationsMenu(LocationsManager locationsManager, ItemManager itemManager)
    {
        this.locationsManager = locationsManager;
        this.itemManager = itemManager;
    }

    public void Show()
    {
        while (true)
        {
            var options = new List<string>();
            options.Add("Create Location");
            if (locationsManager.GetAllLocations().Count > 0)
            {
                options.Add("Edit/Delete Location");
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
                case "Edit/Delete Location":
                    SelectLocationByListMenu();
                    break;
                default:
                    AnsiConsole.WriteLine(nyi);
                    break;
            }
        }
    }

    public Location? CreateLocationMenu()
    {

        AnsiConsole.Clear();
        AnsiConsole.WriteLine("=== Create Location  ===\n");
        
        // Get inputs
        String name = LocationNamePrompt();

        Location? location = locationsManager.CreateLocation(name);
        if (location == null)
        {
            AnsiConsole.MarkupLine("[red]Location could not create[/]");
        }
        return location;

    }                


    void SelectLocationByListMenu()
    {
        Location selectedLocation = AnsiConsole.Prompt(
            new SelectionPrompt<Location>()
                .Title("=== Select an Location ===")
                .PageSize(10)
                .UseConverter(location => location.Name)
                .AddChoices(locationsManager.GetAllLocations())
                .EnableSearch()
        );
        LocationActionMenu(selectedLocation);
    }

    void LocationActionMenu(Location location)
    {
        while (true)
        {
            AnsiConsole.Clear();
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"=== {location.Name} ===")
                    .AddChoices("Update Location", "Delete Location", "Back")
            );

            switch (selection)
            {
                case "Back":
                    return;
                case "Update Location":
                    UpdateLocationMenu(location);
                    break;
                case "Delete Location":
                    Boolean deleted = DeleteLocation(location);
                    if(deleted) return;
                    break;
                default:
                    AnsiConsole.WriteLine(nyi);
                    break;
            }
        }
    }

    void UpdateLocationMenu(Location location)
    {
        AnsiConsole.Clear();
        AnsiConsole.WriteLine("=== Update Location ===\n");
        // Get inputs
        String name = LocationNamePrompt(location.Name, location.Id);
        bool success = locationsManager.UpdateLocation(location.Id, name);
        if (!success)
        {
            AnsiConsole.MarkupLine("[red]Location could not update[/]");
            System.Console.ReadKey(true);
        }
    }

    bool DeleteLocation(Location location)
    {
        if (!itemManager.LocationHasItems(location.Id))
        {
            if (AnsiConsole.Confirm($"Are you sure you want to delete {location.Name}?", false))
            {
                return locationsManager.DeleteLocation(location.Id);
            }

            return false;
        }

        var choices = new List<string>();

        if (locationsManager.GetAllLocations()
            .Any(existingLocation => existingLocation.Id != location.Id))
        {
            choices.Add("Transfer items to existing location");
        }

        choices.Add("Create new location and transfer items");
        choices.Add("Clear location from items");
        choices.Add("Back");

        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"{location.Name} contains items. What would you like to do?")
                .AddChoices(choices)
        );

        switch (selection)
        {
            case "Transfer items to existing location":
                Location? target = SelectExistingLocation(location.Id);
                if (target == null) return false;

                itemManager.AssignItemsToLocation(location.Id, target.Id);
                break;

            case "Create new location and transfer items":
                Location? newLocation = CreateLocationMenu();
                if (newLocation == null) return false;

                itemManager.AssignItemsToLocation(location.Id, newLocation.Id);
                break;

            case "Clear location from items":
                if (!AnsiConsole.Confirm(
                    $"This will make all items in {location.Name} unassigned. Continue?",
                    false))
                {
                    return false;
                }

                itemManager.AssignItemsToLocation(location.Id, null);
                break;

            case "Back":
                return false;
        }

        if (AnsiConsole.Confirm($"Now delete {location.Name}?", false))
        {
            return locationsManager.DeleteLocation(location.Id);
        }

        return false;
    }

    // helpers
    private string LocationNamePrompt(String? defaultValue = null, Guid? locationId = null)
    {
        string name;
        while(true)
        {
            name = defaultValue is not null
            ? PromptNotEmpty("Enter location name:", defaultValue)
            : PromptNotEmpty("Enter location name:");
            if ((locationId == null && locationsManager.LocationNameExists(name)) ||
            (locationsManager.LocationNameExists(name, locationId)))
            {
                AnsiConsole.MarkupLine($"[red]{name} is already a location[/]");
                continue;
            }
            
            return name;
        }
    }

    Location? SelectExistingLocation(Guid locationIdToExclude)
    {
        var locations = locationsManager.GetAllLocations()
            .Where(location => location.Id != locationIdToExclude)
            .ToList();

        if (locations.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No other locations exist.[/]");
            return null;
        }

        return AnsiConsole.Prompt(
            new SelectionPrompt<Location>()
                .Title("=== Select a location ===")
                .PageSize(10)
                .UseConverter(location => location.Name)
                .AddChoices(locations)
                .EnableSearch()
        );
    }

}
