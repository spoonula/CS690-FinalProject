using InventoryApp.Services;

namespace InventoryApp.Tests;

public class BorrowersManagerTests
{
    [Fact]
    public void CreateBorrower_WithUniqueName_CreatesBorrower()
    {
        string dir = TestFileHelper.CreateTestDirectory();
        var manager = new BorrowersManager(TestFileHelper.FilePath(dir, "borrowers.json"));

        var borrower = manager.CreateBorrower("Chris");

        Assert.NotNull(borrower);
        Assert.Equal("Chris", borrower.Name);
        Assert.Single(manager.GetAllBorrowers());
    }

    [Fact]
    public void CreateBorrower_WithDuplicateName_ReturnsNull()
    {
        string dir = TestFileHelper.CreateTestDirectory();
        var manager = new BorrowersManager(TestFileHelper.FilePath(dir, "borrowers.json"));

        manager.CreateBorrower("Chris");
        var duplicate = manager.CreateBorrower("Chris");

        Assert.Null(duplicate);
        Assert.Single(manager.GetAllBorrowers());
    }

    [Fact]
    public void UpdateBorrower_WithDuplicateName_ReturnsFalse()
    {
        string dir = TestFileHelper.CreateTestDirectory();
        var manager = new BorrowersManager(TestFileHelper.FilePath(dir, "borrowers.json"));

        var b1 = manager.CreateBorrower("Chris");
        var b2 = manager.CreateBorrower("Alex");

        Assert.NotNull(b1);
        Assert.NotNull(b2);

        bool result = manager.UpdateBorrower(b2.Id, "Chris");

        Assert.False(result);
    }

    [Fact]
    public void DeleteBorrower_RemovesBorrower()
    {
        string dir = TestFileHelper.CreateTestDirectory();
        var manager = new BorrowersManager(TestFileHelper.FilePath(dir, "borrowers.json"));

        var borrower = manager.CreateBorrower("Chris");
        Assert.NotNull(borrower);

        bool deleted = manager.DeleteBorrower(borrower.Id);

        Assert.True(deleted);
        Assert.Empty(manager.GetAllBorrowers());
    }
}