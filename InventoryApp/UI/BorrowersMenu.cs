namespace InventoryApp;

using Spectre.Console;
using InventoryApp.Services;
using InventoryApp.Models;
using static InventoryApp.UI.PromptHelpers;

class BorrowersMenu
{
    const string nyi = "Not yet implemented";
    private BorrowersManager borrowersManager;

    public BorrowersMenu(BorrowersManager borrowersManager)
    {
        this.borrowersManager = borrowersManager;
    }

    public void Show()
    {
        while (true)
        {
            var options = new List<string>();
            options.Add("Create Borrower");

            if (borrowersManager.GetAllBorrowers().Count > 0)
            {
                options.Add("Edit/Delete Borrower");
            }

            options.Add("Back");

            AnsiConsole.Clear();

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("=== Borrowers ===")
                    .AddChoices(options)
            );

            switch (selection)
            {
                case "Back":
                    return;
                case "Create Borrower":
                    CreateBorrowerMenu();
                    break;
                case "Edit/Delete Borrower":
                    SelectBorrowerByListMenu();
                    break;
                default:
                    AnsiConsole.WriteLine(nyi);
                    break;
            }
        }
    }

    public Borrower? CreateBorrowerMenu()
    {
        AnsiConsole.Clear();
        AnsiConsole.WriteLine("=== Create Borrower ===\n");

        string name = BorrowerNamePrompt();

        Borrower? borrower = borrowersManager.CreateBorrower(name);

        if (borrower == null)
        {
            AnsiConsole.MarkupLine("[red]Borrower could not create[/]");
            Console.ReadKey(true);
        }

        return borrower;
    }

    void SelectBorrowerByListMenu()
    {
        Borrower selectedBorrower = AnsiConsole.Prompt(
            new SelectionPrompt<Borrower>()
                .Title("=== Select a Borrower ===")
                .PageSize(10)
                .UseConverter(borrower => borrower.Name)
                .AddChoices(borrowersManager.GetAllBorrowers())
                .EnableSearch()
        );

        BorrowerActionMenu(selectedBorrower);
    }

    void BorrowerActionMenu(Borrower borrower)
    {
        while (true)
        {
            AnsiConsole.Clear();

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"=== {borrower.Name} ===")
                    .AddChoices("Update Borrower", "Delete Borrower", "Back")
            );

            switch (selection)
            {
                case "Back":
                    return;
                case "Update Borrower":
                    UpdateBorrowerMenu(borrower);
                    break;
                case "Delete Borrower":
                    bool deleted = DeleteBorrower(borrower);
                    if (deleted) return;
                    break;
                default:
                    AnsiConsole.WriteLine(nyi);
                    break;
            }
        }
    }

    void UpdateBorrowerMenu(Borrower borrower)
    {
        AnsiConsole.Clear();
        AnsiConsole.WriteLine("=== Update Borrower ===\n");

        string name = BorrowerNamePrompt(borrower.Name, borrower.Id);

        bool success = borrowersManager.UpdateBorrower(borrower.Id, name);

        if (!success)
        {
            AnsiConsole.MarkupLine("[red]Borrower could not update[/]");
            Console.ReadKey(true);
        }
    }

    bool DeleteBorrower(Borrower borrower)
    {
        if (AnsiConsole.Confirm($"Are you sure you want to delete {borrower.Name}?", defaultValue: false))
        {
            return borrowersManager.DeleteBorrower(borrower.Id);
        }

        return false;
    }

    private string BorrowerNamePrompt(string? defaultValue = null, Guid? borrowerId = null)
    {
        string name;

        while (true)
        {
            name = defaultValue is not null
                ? PromptNotEmpty("Enter borrower name:", defaultValue)
                : PromptNotEmpty("Enter borrower name:");

            if (borrowersManager.BorrowerNameExists(name, borrowerId))
            {
                AnsiConsole.MarkupLine($"[red]{name} is already a borrower[/]");
                continue;
            }

            return name;
        }
    }
}