namespace Domain.Entities
{
    public class OrderStatus
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
