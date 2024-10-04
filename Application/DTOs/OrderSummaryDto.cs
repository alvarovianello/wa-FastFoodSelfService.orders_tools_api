using Domain.Entities;
using Domain.Enums;

namespace Application.DTOs
{
    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string? OrderNumber { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = EnumOrderStatus.Recebido.ToString();
        public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();

        public OrderSummaryDto(Order order, IEnumerable<ProductDto> products)
        {
            Id = order.Id;
            CustomerId = order.CustomerId;
            OrderNumber = order.OrderNumber;
            TotalPrice = order.TotalPrice;
            CreatedAt = order.CreatedAt;
            Status = Enum.GetName(typeof(EnumOrderStatus), order.Status) ?? EnumOrderStatus.Recebido.ToString();
            OrderItems = order.OrderItems.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                TotalPrice = item.TotalPrice,
                PriceItem = products.FirstOrDefault(p => p.Id == item.ProductId)?.Price ?? 0,
                ProductName = products.FirstOrDefault(p => p.Id == item.ProductId)?.Name,
                Description = products.FirstOrDefault(p => p.Id == item.ProductId)?.Description
            }).ToList();
        }
    }
}
