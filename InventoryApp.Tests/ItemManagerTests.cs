using InventoryApp.Services;

namespace InventoryApp.Tests;

public class ItemManagerTests
{
    [Fact]
    public void CreateItem_WithUniqueName_CreatesItem()
    {
        string directory = TestFileHelper.CreateTestDirectory();
        var itemManager = new ItemManager(TestFileHelper.FilePath(directory, "items.json"));

        var item = itemManager.CreateItem("Hammer", "Claw hammer", 12.50m, null);

        Assert.NotNull(item);
        Assert.Equal("Hammer", item.Name);
        Assert.Single(itemManager.GetAllItems());
    }

    [Fact]
    public void CreateItem_WithDuplicateName_ReturnsNull()
    {
        string directory = TestFileHelper.CreateTestDirectory();
        var itemManager = new ItemManager(TestFileHelper.FilePath(directory, "items.json"));

        itemManager.CreateItem("Hammer", "First hammer", 10, null);
        var duplicate = itemManager.CreateItem("Hammer", "Second hammer", 20m, null);

        Assert.Null(duplicate);
        Assert.Single(itemManager.GetAllItems());
    }

    [Fact]
    public void UpdateItem_WithNullLocation_ClearsLocation()
    {
        string directory = TestFileHelper.CreateTestDirectory();
        var itemManager = new ItemManager(TestFileHelper.FilePath(directory, "items.json"));

        Guid locationId = Guid.NewGuid();

        var item = itemManager.CreateItem("Drill", "Cordless drill", 50, locationId);

        Assert.NotNull(item);

        bool updated = itemManager.UpdateItem(
            item.Id,
            item.Name,
            item.Description,
            item.EstimatedValue,
            null
        );

        var updatedItem = itemManager.GetItemById(item.Id);

        Assert.True(updated);
        Assert.NotNull(updatedItem);
        Assert.Null(updatedItem.LocationId);
    }
}