using CsvHelper;
using InventoryApp.Models;
using System.Globalization;

namespace InventoryApp.Services
{
    public class ReportsManager
    {
        private ItemManager itemManager;
        private LocationsManager locationsManager;
        private LoansManager loansManager;
        private BorrowersManager borrowersManager;

        private string reportDirectory;

        public ReportsManager(
            ItemManager itemManager,
            LocationsManager locationsManager,
            LoansManager loansManager,
            BorrowersManager borrowersManager,
            string reportDirectory = "Reports")
        {
            this.itemManager = itemManager;
            this.locationsManager = locationsManager;
            this.loansManager = loansManager;
            this.borrowersManager = borrowersManager;
            this.reportDirectory = reportDirectory;
        }

        public string GenerateInventoryReport(bool includeLoanedItems)
        {
            var items = itemManager.GetAllItems();
            var locations = locationsManager.GetAllLocations();

            if (!includeLoanedItems)
            {
                items = items
                    .Where(item => !loansManager.ItemIsLoaned(item.Id))
                    .ToList();
            }

            var reportRows = items.Select(item =>
            {
                string locationName = "Unassigned";

                if (item.LocationId is Guid locationId)
                {
                    Location? location = locations.FirstOrDefault(l => l.Id == locationId);
                    if (location != null)
                    {
                        locationName = location.Name;
                    }
                }

                bool isLoaned = loansManager.ItemIsLoaned(item.Id);

                return new
                {
                    ItemName = item.Name,
                    Location = locationName,
                    EstimatedValue = item.EstimatedValue.ToString("F2"),
                    LoanStatus = isLoaned ? "Loaned" : "Available"
                };
            });

            return WriteReport(reportRows, "inventory-report");
        }

        public string GenerateLoanedItemsReport()
        {
            var activeLoans = loansManager.GetActiveLoans();

            var reportRows = activeLoans.Select(loan =>
            {
                Item? item = itemManager.GetItemById(loan.ItemId);
                Borrower? borrower = borrowersManager.GetBorrowerById(loan.BorrowerId);

                bool isOverdue = loan.ExpectedReturnDate.Date < DateTime.Today;

                return new
                {
                    ItemName = item != null ? item.Name : "Unknown item",
                    BorrowerName = borrower != null ? borrower.Name : "Unknown borrower",
                    LoanDate = loan.LoanDate.ToShortDateString(),
                    ExpectedReturnDate = loan.ExpectedReturnDate.ToShortDateString(),
                    OverdueStatus = isOverdue ? "Overdue" : "Not overdue"
                };
            });

            return WriteReport(reportRows, "loaned-items-report");
        }

        private string WriteReport<T>(IEnumerable<T> rows, string reportName)
        {
            Directory.CreateDirectory(reportDirectory);

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HHmmssfff");
            string filePath = Path.Combine(reportDirectory, $"{reportName}-{timestamp}.csv");

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(rows);
            }

            return filePath;
        }
    }
}