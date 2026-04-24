namespace InventoryApp;

using Spectre.Console;
using InventoryApp.Services;
using InventoryApp.Models;
using static InventoryApp.UI.PromptHelpers;
using InventoryApp.UI;
using CsvHelper;
using System.Globalization;

class App
{

    const String nyi = "Not yet implemented";
    private ItemManager itemManager = new ItemManager();
    private LocationsManager locationsManager = new LocationsManager();
    private BorrowersManager borrowersManager = new BorrowersManager();
    private LoansManager loansManager = new LoansManager();
    private ReportsManager reportsManager;

    public App()
    {
        this.reportsManager = new ReportsManager(
            itemManager,
            locationsManager,
            loansManager,
            borrowersManager
        );
    }

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
                    var itemsMenu = new ItemsMenu(itemManager, locationsManager, borrowersManager, loansManager);
                    itemsMenu.Show();
                    break;
                case "Locations":
                    var locationsMenu = new LocationsMenu(locationsManager);
                    locationsMenu.Show();     
                    break;   
                case "Borrowers":
                    var borrowersMenu = new BorrowersMenu(borrowersManager);
                    borrowersMenu.Show();
                    break;
                case "Reports":
                    var reportsMenu = new ReportsMenu(reportsManager);
                    reportsMenu.Show();
                    break;   
                default:
                    AnsiConsole.WriteLine(nyi);
                    break;
            }
        }
    }
}
