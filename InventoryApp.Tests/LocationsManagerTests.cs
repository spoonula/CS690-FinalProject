using InventoryApp.Services;

namespace InventoryApp.Tests;

public class LocationsManagerTests
{
    [Fact]
    public void CreateLocation_WithUniqueName_CreatesLocation()
    {
        string dir = TestFileHelper.CreateTestDirectory();
        var manager = new LocationsManager(TestFileHelper.FilePath(dir, "locations.json"));

        var location = manager.CreateLocation("Garage");

        Assert.NotNull(location);
        Assert.Equal("Garage", location.Name);
        Assert.Single(manager.GetAllLocations());
    }

    [Fact]
    public void CreateLocation_WithDuplicateName_ReturnsNull()
    {
        string dir = TestFileHelper.CreateTestDirectory();
        var manager = new LocationsManager(TestFileHelper.FilePath(dir, "locations.json"));

        manager.CreateLocation("Garage");
        var duplicate = manager.CreateLocation("Garage");

        Assert.Null(duplicate);
        Assert.Single(manager.GetAllLocations());
    }

    [Fact]
    public void UpdateLocation_WithDuplicateName_ReturnsFalse()
    {
        string dir = TestFileHelper.CreateTestDirectory();
        var manager = new LocationsManager(TestFileHelper.FilePath(dir, "locations.json"));

        var l1 = manager.CreateLocation("Garage");
        var l2 = manager.CreateLocation("Basement");

        Assert.NotNull(l1);
        Assert.NotNull(l2);

        bool result = manager.UpdateLocation(l2.Id, "Garage");

        Assert.False(result);
    }

    [Fact]
    public void DeleteLocation_RemovesLocation()
    {
        string dir = TestFileHelper.CreateTestDirectory();
        var manager = new LocationsManager(TestFileHelper.FilePath(dir, "locations.json"));

        var location = manager.CreateLocation("Garage");
        Assert.NotNull(location);

        bool deleted = manager.DeleteLocation(location.Id);

        Assert.True(deleted);
        Assert.Empty(manager.GetAllLocations());
    }
}