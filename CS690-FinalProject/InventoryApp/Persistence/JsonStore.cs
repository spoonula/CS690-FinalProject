using System.Text.Json;

namespace InventoryApp.Persistence
{
    public class JsonStore<T>
    {
        private readonly string _filePath;

        public JsonStore(string filePath)
        {
            _filePath = filePath;
        }

        public List<T> Load()
        {
            // Gets data from the JSON file and loads it into a List of the proper class
            EnsureFileExists();

            var json = File.ReadAllText(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<T>();
            }

            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        public void Save(List<T> items)
        {
            // Writes the list of objects to JSON
            EnsureFileExists();
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(items, options);
            File.WriteAllText(_filePath, json);
        }

        private void EnsureFileExists()
        {
            // Helper to make sure the file exists and create it if it doesn't exist
            var directory = Path.GetDirectoryName(_filePath);

            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }
    }
}