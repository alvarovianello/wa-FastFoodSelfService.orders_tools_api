using Application.DTOs;

namespace Application.Interfaces.UseCases
{
    public interface ICreateOrder
    {
        Task<OrderDto> ExecuteAsync(CreateOrderRequest request);
    }
}
