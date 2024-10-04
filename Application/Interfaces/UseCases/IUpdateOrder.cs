using Application.DTOs;

namespace Application.Interfaces.UseCases
{
    public interface IUpdateOrder
    {
        Task<OrderDto> ExecuteAsync(UpdateOrderRequest request);
    }
}
