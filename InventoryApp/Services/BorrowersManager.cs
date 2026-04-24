using InventoryApp.Models;
using InventoryApp.Persistence;
using System.Linq;

namespace InventoryApp.Services
{
    public class BorrowersManager
    {
        private JsonStore<Borrower> store;
        private List<Borrower> borrowers;

        public BorrowersManager(string filePath = "Data/borrowers.json")
        {
            store = new JsonStore<Borrower>(filePath);
            borrowers = store.Load();
        }

        public List<Borrower> GetAllBorrowers()
        {
            return borrowers;
        }

        public Borrower? GetBorrowerById(Guid id)
        {
            return borrowers.FirstOrDefault(borrower => borrower.Id == id);
        }

        public Borrower? CreateBorrower(string name)
        {
            if (BorrowerNameExists(name))
            {
                return null;
            }

            Borrower borrower = new Borrower();
            borrower.Id = Guid.NewGuid();
            borrower.Name = name;

            borrowers.Add(borrower);
            borrowers.Sort((x, y) => x.Name.CompareTo(y.Name));
            Save();

            return borrower;
        }

        public bool UpdateBorrower(Guid id, string name)
        {
            Borrower? borrower = GetBorrowerById(id);

            if (borrower == null)
            {
                return false;
            }

            if (BorrowerNameExists(name, id))
            {
                return false;
            }

            borrower.Name = name;
            borrowers.Sort((x, y) => x.Name.CompareTo(y.Name));
            Save();

            return true;
        }

        public bool DeleteBorrower(Guid id)
        {
            Borrower? borrower = GetBorrowerById(id);

            if (borrower == null)
            {
                return false;
            }

            borrowers.Remove(borrower);
            Save();

            return true;
        }

        public bool BorrowerNameExists(string name, Guid? ignoreId = null)
        {
            return borrowers.Any(borrower =>
                borrower.Name == name &&
                (ignoreId == null || borrower.Id != ignoreId));
        }

        private void Save()
        {
            store.Save(borrowers);
        }
    }
}