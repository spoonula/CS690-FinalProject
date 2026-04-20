namespace InventoryApp;

using Spectre.Console;
using InventoryApp.Services;
using InventoryApp.Models;
using System.Globalization;
using CsvHelper;

class ReportsMenu
{
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
                    .AddChoices("Inventory Report",/* "Loan Report", */"Back")
            );

            switch (selection)
            {
                case "Back":
                    return;
                case "Inventory Report":
                    InventoryReportMenu();
                    break;
                case "Loan Report":
                    break;
                default:
                    break;
            }
        }
    }

    void InventoryReportMenu()
    {
        if(AnsiConsole.Confirm("Ready to Generate Inventory Report?"))
        {
            GenerateInventoryReport();
        }      
    }

    private void GenerateInventoryReport()
    {
        var items = itemManager.GetAllItems();
        var locations = locationsManager.GetAllLocations();

        var reportRows = items.Select(item =>
        {
            var locationName = "Unassigned";
            if (item.LocationId is Guid locationId)
            {
                var location = locations.FirstOrDefault(l => l.Id == locationId);
                if (location != null)
                {
                    locationName = location.Name;
                }
            }
            return new
            {
                ItemName = item.Name,
                Location = locationName,
                EstimatedValue = item.EstimatedValue.ToString("F2")
            };
        });

       WriteReport(reportRows, "inventory-report");
    }

    private void WriteReport<T>(IEnumerable<T> rows, string reportName)
    {
        Directory.CreateDirectory("Reports");
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd-HHmm");
        var filePath = $"Reports/{reportName}-{timestamp}.csv";

        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvHelper.CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(rows);
        }
        Spectre.Console.AnsiConsole.MarkupLine($"[green]Report generated:[/] {filePath}");
        Spectre.Console.AnsiConsole.WriteLine("Any key to continue");
        System.Console.ReadKey(true);
    }
}