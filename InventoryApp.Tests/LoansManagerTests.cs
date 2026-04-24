using InventoryApp.Services;

namespace InventoryApp.Tests;

public class LoansManagerTests
{
    [Fact]
    public void CreateLoan_WhenItemIsAvailable_CreatesLoan()
    {
        string directory = TestFileHelper.CreateTestDirectory();
        var loansManager = new LoansManager(TestFileHelper.FilePath(directory, "loans.json"));

        Guid itemId = Guid.NewGuid();
        Guid borrowerId = Guid.NewGuid();

        var loan = loansManager.CreateLoan(
            itemId,
            borrowerId,
            DateTime.Today,
            DateTime.Today.AddDays(7)
        );

        Assert.NotNull(loan);
        Assert.Equal(itemId, loan.ItemId);
        Assert.Equal(borrowerId, loan.BorrowerId);
        Assert.True(loansManager.ItemIsLoaned(itemId));
    }

    [Fact]
    public void CreateLoan_WhenItemAlreadyLoaned_ReturnsNull()
    {
        string directory = TestFileHelper.CreateTestDirectory();
        var loansManager = new LoansManager(TestFileHelper.FilePath(directory, "loans.json"));

        Guid itemId = Guid.NewGuid();

        loansManager.CreateLoan(itemId, Guid.NewGuid(), DateTime.Today, DateTime.Today.AddDays(7));

        var secondLoan = loansManager.CreateLoan(
            itemId,
            Guid.NewGuid(),
            DateTime.Today,
            DateTime.Today.AddDays(14)
        );

        Assert.Null(secondLoan);
    }

    [Fact]
    public void MarkReturned_WhenLoanExists_MarksItemAvailable()
    {
        string directory = TestFileHelper.CreateTestDirectory();
        var loansManager = new LoansManager(TestFileHelper.FilePath(directory, "loans.json"));

        Guid itemId = Guid.NewGuid();

        loansManager.CreateLoan(itemId, Guid.NewGuid(), DateTime.Today, DateTime.Today.AddDays(7));

        bool returned = loansManager.MarkReturned(itemId, DateTime.Today.AddDays(2));

        Assert.True(returned);
        Assert.False(loansManager.ItemIsLoaned(itemId));
    }
}