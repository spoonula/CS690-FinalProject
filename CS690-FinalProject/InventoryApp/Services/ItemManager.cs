using InventoryApp.Models;
using InventoryApp.Persistence;
using System.Linq;

namespace InventoryApp.Services
{
    public class ItemManager
    {
        private JsonStore<Item> store;
        private List<Item> items;

        public ItemManager()
        {
            store = new JsonStore<Item>("Data/items.json");
            items = store.Load();
        }

        public List<Item> GetAllItems()
        {
            return items;
        }

        public Item? GetItemById(Guid id)
        {
            return items.FirstOrDefault(item => item.Id == id);
        }

        public Item CreateItem(string name, string description, decimal estimatedValue, Guid? locationId)
        {
            Item item = new Item();
            item.Id = Guid.NewGuid();
            item.Name = name;
            item.Description = description;
            item.EstimatedValue = estimatedValue;
            item.LocationId = locationId;
            items.Add(item);
            items.Sort((x, y) => x.Name.CompareTo(y.Name));
            Save();

            return item;
        }

        public bool UpdateItem(Guid id, string name, string description, decimal estimatedValue, Guid? locationId)
        {
            Item? item = GetItemById(id);

            if (item == null)
            {
                return false;
            }

            item.Name = name;
            item.Description = description;
            item.EstimatedValue = estimatedValue;
            item.LocationId = locationId;
            items.Sort((x, y) => x.Name.CompareTo(y.Name));
            Save();
            return true;
        }

        public bool DeleteItem(Guid id)
        {
            Item? item = GetItemById(id);

            if (item == null)
            {
                return false;
            }

            items.Remove(item);
            Save();

            return true;
        }

        private void Save()
        {
            store.Save(items);
        }
    }
}