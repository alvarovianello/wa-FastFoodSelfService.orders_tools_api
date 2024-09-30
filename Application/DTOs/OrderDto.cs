using Domain.Entities;
using Domain.Enums;

namespace Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string? OrderNumber { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public EnumOrderStatus Status { get; set; } = EnumOrderStatus.Recebido;
        public string StatusPagamento { get; set; } = EnumStatusPayment.Pending.ToString();
        public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public CustomerDto? Customer { get; set; }

        public OrderDto(Order order, string paymentStatus, IEnumerable<ProductDto> products, CustomerDto? customer = null)
        {
            Id = order.Id;
            CustomerId = order.CustomerId;
            OrderNumber = order.OrderNumber;
            TotalPrice = order.TotalPrice;
            CreatedAt = order.CreatedAt;
            Status = (EnumOrderStatus)order.Status;
            StatusPagamento = paymentStatus;
            OrderItems = order.OrderItems.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                TotalPrice = item.TotalPrice,
                PriceItem = products.FirstOrDefault(p => p.Id == item.ProductId)?.Price ?? 0,
                ProductName = products.FirstOrDefault(p => p.Id == item.ProductId)?.Name
            }).ToList();
            Customer = customer;
        }
    }
}
