namespace InventoryApp.Tests;

public static class TestFileHelper
{
    public static string CreateTestDirectory()
    {
        string directory = Path.Combine(
            Path.GetTempPath(),
            "InventoryAppTests",
            Guid.NewGuid().ToString()
        );

        Directory.CreateDirectory(directory);
        return directory;
    }

    public static string FilePath(string directory, string fileName)
    {
        return Path.Combine(directory, fileName);
    }
}