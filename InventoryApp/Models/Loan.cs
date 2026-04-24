namespace InventoryApp.Models
{
    public class Loan
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public Guid BorrowerId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
    }
}