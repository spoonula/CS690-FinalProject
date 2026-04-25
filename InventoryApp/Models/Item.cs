namespace InventoryApp.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal EstimatedValue { get; set; }
        public Guid? LocationId { get; set; }
    }
}