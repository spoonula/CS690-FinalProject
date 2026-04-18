namespace InventoryApp;

using Spectre.Console;
using InventoryApp.Services;
using InventoryApp.Models;
using static InventoryApp.UI.PromptHelpers;
using InventoryApp.UI;

class App
{

    const String nyi = "Not yet implemented";
    private ItemManager itemManager = new ItemManager();

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
                    var itemsMenu = new ItemsMenu(itemManager);
                    itemsMenu.Show();
                    break;           
                default:
                    AnsiConsole.WriteLine(nyi);
                    break;
            }
        }
    }
}
