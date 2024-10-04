using Application.DTOs;
using Domain.Enums;

namespace Application.Interfaces.UseCases
{
    public interface IGetOrderByStatus
    {
        Task<IEnumerable<OrderDto>> ExecuteAsync(EnumOrderStatus status);
    }
}
