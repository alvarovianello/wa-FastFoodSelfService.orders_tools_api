using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UpdateOrderRequest
    {
        public int OrderId { get; set; }
        public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}
