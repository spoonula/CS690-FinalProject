namespace InventoryApp;

using Spectre.Console;
using InventoryApp.Services;
using InventoryApp.Models;
using static InventoryApp.UI.PromptHelpers;
using System.Globalization;

class ItemsMenu
{

    const String nyi = "Not yet implemented";
    private ItemManager itemManager;
    private LocationsManager locationsManager;
    private BorrowersManager borrowersManager;
    private LoansManager loansManager;

    public ItemsMenu(
        ItemManager itemManager,
        LocationsManager locationsManager,
        BorrowersManager borrowersManager,
        LoansManager loansManager) 
    {
        this.itemManager = itemManager;
        this.locationsManager = locationsManager;
        this.borrowersManager = borrowersManager;
        this.loansManager = loansManager;
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
        String name = ItemNamePrompt();
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
        else
        {
            AnsiConsole.MarkupLine("[red]Item could not create[/]");
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
            options.Add("Remove Location From Item");
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
                    LocationsMenu locationsMenu = new LocationsMenu(locationsManager, itemManager);
                    l = locationsMenu.CreateLocationMenu();
                break;
            case "Back":
                break;
            case "Remove Location From Item":
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
        if (AnsiConsole.Confirm("Filter by location?", defaultValue: false))
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
            var options = new List<string>
            {
                "View Details",
                "Update Item",
                "Delete Item",
                "Assign / Change Location"
            };

            if (loansManager.ItemIsLoaned(item.Id))
            {
                options.Add("Mark as Returned");
            }
            else
            {
                options.Add("Mark as Loaned");
            }

            options.Add("Back");

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"=== {item.Name} ===")
                    .AddChoices(options)
            );

            switch (selection)
            {
                case "Back":
                    return;
                case "View Details":
                    PrintItemDetails(item);
                    break;
                case "Update Item":
                    UpdateItemMenu(item);
                    break;
                case "Delete Item":
                    Boolean deleted = DeleteItem(item);
                    if(deleted) return;
                    break;
                case "Mark as Loaned":
                    MarkItemLoanedMenu(item);
                    break;
                case "Mark as Returned":
                    MarkItemReturnedMenu(item);
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

    void UpdateItemMenu(Item item)
    {

        AnsiConsole.Clear();
        AnsiConsole.WriteLine("=== Update Item ===\n");
        // Get Item inputs
        String name = ItemNamePrompt(item.Name, item.Id);
        String description = PromptNotEmpty("Enter item description:", item.Description);
        decimal value = PromptDecimal("Enter estimated value:", false, item.EstimatedValue.ToString());
        AnsiConsole.WriteLine("");
        bool success = itemManager.UpdateItem(item.Id, name, description, value, item.LocationId);
        if (!success)
        {
            AnsiConsole.MarkupLine("[red]Item could not update[/]");
            System.Console.ReadKey(true);
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
        Loan? loan = loansManager.GetActiveLoanByItemId(item.Id);
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
        AnsiConsole.MarkupLine($"[bold]Value: [/] {item.EstimatedValue.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
        if (loan == null)
        {
            AnsiConsole.MarkupLine("[bold]Loan Status: [/] Available");
        }
        else
        {
            Borrower? borrower = borrowersManager.GetBorrowerById(loan.BorrowerId);
            string borrowerName = borrower != null ? borrower.Name : "Unknown borrower";

            AnsiConsole.MarkupLine("[bold]Loan Status: [/] [yellow]Loaned[/]");
            AnsiConsole.MarkupLine($"[bold]Borrower: [/] {borrowerName}");
            AnsiConsole.MarkupLine($"[bold]Loan Date: [/] {loan.LoanDate.ToShortDateString()}");
            AnsiConsole.MarkupLine($"[bold]Expected Return: [/] {loan.ExpectedReturnDate.ToShortDateString()}");
        }
        AnsiConsole.WriteLine("\nAny key to return");
        Console.ReadKey(true);
    }

    void MarkItemLoanedMenu(Item item)
    {
        AnsiConsole.Clear();
        AnsiConsole.WriteLine($"=== Loan {item.Name} ===\n");

        Borrower? borrower = null;
        borrower = BorrowerSelectMenu(borrower, true);

        if (borrower == null)
        {
            return;
        }

        DateTime loanDate = PromptDate("Enter loan date:", DateTime.Today.ToShortDateString());
        DateTime expectedReturnDate = PromptDate("Enter expected return date:");

        Loan? loan = loansManager.CreateLoan(
            item.Id,
            borrower.Id,
            loanDate,
            expectedReturnDate
        );

        if (loan == null)
        {
            AnsiConsole.MarkupLine("[red]Item could not be marked as loaned[/]");
            Console.ReadKey(true);
        }

        itemManager.UpdateItem(
            item.Id,
            item.Name,
            item.Description,
            item.EstimatedValue,
            null
        );
    }

    void MarkItemReturnedMenu(Item item)
    {
        AnsiConsole.Clear();
        AnsiConsole.WriteLine($"=== Return {item.Name} ===\n");

        Loan? loan = loansManager.GetActiveLoanByItemId(item.Id);

        if (loan == null)
        {
            AnsiConsole.MarkupLine("[red]This item is not currently loaned[/]");
            Console.ReadKey(true);
            return;
        }

        Borrower? borrower = borrowersManager.GetBorrowerById(loan.BorrowerId);
        string borrowerName = borrower != null ? borrower.Name : "Unknown borrower";

        AnsiConsole.MarkupLine($"[bold]Borrower:[/] {borrowerName}");
        AnsiConsole.MarkupLine($"[bold]Loan Date:[/] {loan.LoanDate.ToShortDateString()}");
        AnsiConsole.MarkupLine($"[bold]Expected Return:[/] {loan.ExpectedReturnDate.ToShortDateString()}");
        AnsiConsole.WriteLine();

        if (AnsiConsole.Confirm("Mark this item as returned?", defaultValue: true))
        {
            DateTime returnedDate = PromptDate(
                "Enter returned date:",
                DateTime.Today.ToShortDateString()
            );

            bool success = loansManager.MarkReturned(item.Id, returnedDate);

            if (!success)
            {
                AnsiConsole.MarkupLine("[red]Item could not be marked as returned[/]");
                Console.ReadKey(true);
                return;
            }

            if (AnsiConsole.Confirm("Would you like to set a return location?", defaultValue: true))
            {
                Location? location = SetReturnLocationForItem(item);

                if (location != null)
                {
                    itemManager.UpdateItem(
                        item.Id,
                        item.Name,
                        item.Description,
                        item.EstimatedValue,
                        location.Id
                    );
                }
            }
        }
    }

    public void SetReturnLocationForItem(Item item)
    {
        if (AnsiConsole.Confirm("Would you like to set a return location?", defaultValue: true))
        {
            Location? location = LocationSelectMenu(null, true, false);

            if (location != null)
            {
                itemManager.UpdateItem(
                    item.Id,
                    item.Name,
                    item.Description,
                    item.EstimatedValue,
                    location.Id
                );
            }
        }
    }

    bool DeleteItem(Item item)
    {
        Loan? activeLoan = loansManager.GetActiveLoanByItemId(item.Id);

        string message;

        if (activeLoan == null)
        {
            message = $"Are you sure you want to delete {item.Name}?";
        }
        else
        {
            Borrower? borrower = borrowersManager.GetBorrowerById(activeLoan.BorrowerId);
            string borrowerName = borrower != null ? borrower.Name : "Unknown borrower";

            message = $"{item.Name} is currently loaned to {borrowerName}. Are you sure you want to delete it?";
        }

        if (AnsiConsole.Confirm(message, defaultValue: false))
        {
            loansManager.DeleteLoansForItem(item.Id);
            return itemManager.DeleteItem(item.Id);
        }

        return false;
    }

    Borrower? BorrowerSelectMenu(Borrower? currentBorrower, bool allowCreate)
    {
        Borrower? b = currentBorrower;

        var borrowers = borrowersManager.GetAllBorrowers();

        var options = borrowers.Select(borrower => borrower.Name).ToList();

        if (allowCreate)
        {
            options.Add("Create New Borrower");
        }

        options.Add("Back");

        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("=== Select a borrower === ")
                .AddChoices(options)
        );

        switch (selection)
        {
            case "Create New Borrower":
                BorrowersMenu borrowersMenu = new BorrowersMenu(borrowersManager, loansManager, itemManager, locationsManager);
                b = borrowersMenu.CreateBorrowerMenu();
                break;

            case "Back":
                break;

            default:
                int index = options.IndexOf(selection);
                b = borrowers[index];
                break;
        }

        return b;
    }

    // helpers
    private string ItemNamePrompt(String? defaultValue = null, Guid? itemId = null)
    {
        string name;
        while(true)
        {
            name = defaultValue is not null
            ? PromptNotEmpty("Enter item name:", defaultValue)
            : PromptNotEmpty("Enter item name:");
            if ((itemManager.ItemNameExists(name) && itemId == null) ||
            (itemManager.ItemNameExists(name, itemId)))
            {
                AnsiConsole.MarkupLine($"[red]{name} is already an item[/]");
                continue;
            }
            return name;
        }
    }

}
