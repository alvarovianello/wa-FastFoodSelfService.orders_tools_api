using Domain.Enums;

namespace Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
