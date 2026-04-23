using InventoryApp.Models;
using InventoryApp.Persistence;
using System.Linq;

namespace InventoryApp.Services
{
    public class LocationsManager
    {
        private JsonStore<Location> store;
        private List<Location> locations;

        public LocationsManager()
        {
            store = new JsonStore<Location>("Data/locations.json");
            locations = store.Load();
        }

        public List<Location> GetAllLocations()
        {
            return locations;
        }

        public Location? GetLocationById(Guid id)
        {
            return locations.FirstOrDefault(location => location.Id == id);
        }

        public Location CreateLocation(string name)
        {
            if (LocationNameExists(name))
            {
                // no dups!
                return null;
            }

            Location location = new Location();
            location.Id = Guid.NewGuid();
            location.Name = name;

            locations.Add(location);
            locations.Sort((x, y) => x.Name.CompareTo(y.Name));
            Save();

            return location;
        }

        public bool UpdateLocation(Guid id, string name)
        {
            Location? location = GetLocationById(id);

            if (location == null)
            {
                return false;
            }

            if (LocationNameExists(name, id))
            {
                // no dups!
                return false;
            }

            location.Name = name;
            locations.Sort((x, y) => x.Name.CompareTo(y.Name));
            Save();
            return true;
        }

        public bool DeleteLocation(Guid id)
        {
            Location? location = GetLocationById(id);

            if (location == null)
            {
                return false;
            }

            locations.Remove(location);
            Save();

            return true;
        }

        public bool LocationNameExists(string name, Guid? ignoreId = null)
{
            return locations.Any(location =>
                location.Name == name &&
                (ignoreId == null || location.Id != ignoreId));
        }

        private void Save()
        {
            store.Save(locations);
        }
    }
}