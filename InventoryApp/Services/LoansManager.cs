using InventoryApp.Models;
using InventoryApp.Persistence;

namespace InventoryApp.Services
{
    public class LoansManager
    {
        private JsonStore<Loan> store;
        private List<Loan> loans;

        public LoansManager(string filePath = "Data/loans.json")
        {
            store = new JsonStore<Loan>(filePath);
            loans = store.Load();
        }

        public Loan? GetActiveLoanByItemId(Guid itemId)
        {
            return loans.FirstOrDefault(loan =>
                loan.ItemId == itemId &&
                loan.ReturnedDate == null);
        }

        public bool ItemIsLoaned(Guid itemId)
        {
            return GetActiveLoanByItemId(itemId) != null;
        }

        public Loan? CreateLoan(Guid itemId, Guid borrowerId, DateTime loanDate, DateTime expectedReturnDate)
        {
            if (ItemIsLoaned(itemId))
            {
                return null;
            }

            Loan loan = new Loan();
            loan.Id = Guid.NewGuid();
            loan.ItemId = itemId;
            loan.BorrowerId = borrowerId;
            loan.LoanDate = loanDate.Date;
            loan.ExpectedReturnDate = expectedReturnDate.Date;

            loans.Add(loan);
            Save();

            return loan;
        }

        public bool MarkReturned(Guid itemId, DateTime returnedDate)
        {
            Loan? loan = GetActiveLoanByItemId(itemId);

            if (loan == null)
            {
                return false;
            }

            loan.ReturnedDate = returnedDate.Date;
            Save();

            return true;
        }

        public bool DeleteLoansForItem(Guid itemId)
        {
            loans.RemoveAll(loan => loan.ItemId == itemId);
            Save();
            return true;
        }

        public List<Loan> GetAllLoans()
        {
            return loans;
        }

        public List<Loan> GetActiveLoans()
        {
            return loans
                .Where(loan => loan.ReturnedDate == null)
                .ToList();
        }

        private void Save()
        {
            store.Save(loans);
        }
    }
}