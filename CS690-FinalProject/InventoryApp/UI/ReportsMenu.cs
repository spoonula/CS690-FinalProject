namespace InventoryApp;

using Spectre.Console;
using InventoryApp.Services;
using InventoryApp.Models;

class ReportsMenu
{

    const String nyi = "Not yet implemented";
    private ItemManager itemManager;
    private LocationsManager locationsManager;

    public ReportsMenu(ItemManager itemManager, LocationsManager locationsManager) 
    {
        this.itemManager = itemManager;
        this.locationsManager = locationsManager;
    }

    public void Show()
    {
        while (true)
        {
            AnsiConsole.Clear();
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("=== Items ===")
                    .AddChoices("Inventory Report", "Loan Report", "Back")
            );

            switch (selection)
            {
                case "Back":
                    return;
                case "Inventory Report":
                    break;
                case "Loan Report":
                    break;
                default:
                    AnsiConsole.WriteLine(nyi);
                    break;
            }
        }
    }
}