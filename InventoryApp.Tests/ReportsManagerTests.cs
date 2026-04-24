using InventoryApp.Services;

namespace InventoryApp.Tests;

public class ReportsManagerTests
{
    [Fact]
    public void GenerateInventoryReport_WhenLoanedItemsExcluded_DoesNotIncludeLoanedItem()
    {
        string directory = TestFileHelper.CreateTestDirectory();

        var itemManager = new ItemManager(TestFileHelper.FilePath(directory, "items.json"));
        var locationsManager = new LocationsManager(TestFileHelper.FilePath(directory, "locations.json"));
        var loansManager = new LoansManager(TestFileHelper.FilePath(directory, "loans.json"));
        var borrowersManager = new BorrowersManager(TestFileHelper.FilePath(directory, "borrowers.json"));

        var hammer = itemManager.CreateItem("Hammer", "Tool", 10m, null);
        var drill = itemManager.CreateItem("Drill", "Tool", 50m, null);
        var borrower = borrowersManager.CreateBorrower("Chris");

        Assert.NotNull(hammer);
        Assert.NotNull(drill);
        Assert.NotNull(borrower);

        loansManager.CreateLoan(drill.Id, borrower.Id, DateTime.Today, DateTime.Today.AddDays(7));

        var reportsManager = new ReportsManager(
            itemManager,
            locationsManager,
            loansManager,
            borrowersManager,
            TestFileHelper.FilePath(directory, "Reports")
        );

        string reportPath = reportsManager.GenerateInventoryReport(false);
        string csv = File.ReadAllText(reportPath);

        Assert.Contains("Hammer", csv);
        Assert.DoesNotContain("Drill", csv);
    }

    [Fact]
    public void GenerateInventoryReport_WhenLoanedItemsIncluded_IncludesLoanedItem()
    {
        string directory = TestFileHelper.CreateTestDirectory();

        var itemManager = new ItemManager(TestFileHelper.FilePath(directory, "items.json"));
        var locationsManager = new LocationsManager(TestFileHelper.FilePath(directory, "locations.json"));
        var loansManager = new LoansManager(TestFileHelper.FilePath(directory, "loans.json"));
        var borrowersManager = new BorrowersManager(TestFileHelper.FilePath(directory, "borrowers.json"));

        var drill = itemManager.CreateItem("Drill", "Tool", 50m, null);
        var borrower = borrowersManager.CreateBorrower("Chris");

        Assert.NotNull(drill);
        Assert.NotNull(borrower);

        loansManager.CreateLoan(drill.Id, borrower.Id, DateTime.Today, DateTime.Today.AddDays(7));

        var reportsManager = new ReportsManager(
            itemManager,
            locationsManager,
            loansManager,
            borrowersManager,
            TestFileHelper.FilePath(directory, "Reports")
        );

        string reportPath = reportsManager.GenerateInventoryReport(true);
        string csv = File.ReadAllText(reportPath);

        Assert.Contains("Drill", csv);
        Assert.Contains("Loaned", csv);
    }

    [Fact]
    public void GenerateLoanedItemsReport_IncludesBorrowerAndOverdueStatus()
    {
        string directory = TestFileHelper.CreateTestDirectory();

        var itemManager = new ItemManager(TestFileHelper.FilePath(directory, "items.json"));
        var locationsManager = new LocationsManager(TestFileHelper.FilePath(directory, "locations.json"));
        var loansManager = new LoansManager(TestFileHelper.FilePath(directory, "loans.json"));
        var borrowersManager = new BorrowersManager(TestFileHelper.FilePath(directory, "borrowers.json"));

        var item = itemManager.CreateItem("Drill", "Tool", 50m, null);
        var borrower = borrowersManager.CreateBorrower("Chris");

        Assert.NotNull(item);
        Assert.NotNull(borrower);

        loansManager.CreateLoan(
            item.Id,
            borrower.Id,
            DateTime.Today.AddDays(-10),
            DateTime.Today.AddDays(-1)
        );

        var reportsManager = new ReportsManager(
            itemManager,
            locationsManager,
            loansManager,
            borrowersManager,
            TestFileHelper.FilePath(directory, "Reports")
        );

        string reportPath = reportsManager.GenerateLoanedItemsReport();
        string csv = File.ReadAllText(reportPath);

        Assert.Contains("Drill", csv);
        Assert.Contains("Chris", csv);
        Assert.Contains("Overdue", csv);
    }
}