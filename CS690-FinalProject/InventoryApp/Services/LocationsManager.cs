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
            Location location = new Location();
            location.Id = Guid.NewGuid();
            location.Name = name;

            locations.Add(location);
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

            location.Name = name;

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

        private void Save()
        {
            store.Save(locations);
        }
    }
}