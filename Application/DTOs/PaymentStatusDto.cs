using Domain.Enums;

namespace Application.DTOs
{
    public class PaymentStatusDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; }
    }
}
