namespace InventoryApp;

using Spectre.Console;
using InventoryApp.Services;

class ReportsMenu
{
    private ReportsManager reportsManager;

    public ReportsMenu(ReportsManager reportsManager)
    {
        this.reportsManager = reportsManager;
    }

    public void Show()
    {
        while (true)
        {
            AnsiConsole.Clear();

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("=== Reports ===")
                    .AddChoices(
                        "Inventory Report",
                        "Loaned Items Report",
                        "Back"
                    )
            );

            switch (selection)
            {
                case "Back":
                    return;

                case "Inventory Report":
                    InventoryReportMenu();
                    break;

                case "Loaned Items Report":
                    LoanedItemsReportMenu();
                    break;
            }
        }
    }

    void InventoryReportMenu()
    {
        bool includeLoanedItems = AnsiConsole.Confirm(
            "Include loaned items in the inventory report?",
            defaultValue: true
        );

        if (AnsiConsole.Confirm("Ready to generate inventory report?"))
        {
            string filePath = reportsManager.GenerateInventoryReport(includeLoanedItems);
            ReportGeneratedMessage(filePath);
        }
    }

    void LoanedItemsReportMenu()
    {
        if (AnsiConsole.Confirm("Ready to generate loaned items report?"))
        {
            string filePath = reportsManager.GenerateLoanedItemsReport();
            ReportGeneratedMessage(filePath);
        }
    }

    void ReportGeneratedMessage(string filePath)
    {
        AnsiConsole.MarkupLine($"[green]Report generated:[/] {filePath}");
        AnsiConsole.WriteLine("Any key to continue");
        Console.ReadKey(true);
    }
}