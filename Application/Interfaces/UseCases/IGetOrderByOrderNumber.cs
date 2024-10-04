using Application.DTOs;

namespace Application.Interfaces.UseCases
{
    public interface IGetOrderByOrderNumber
    {
        Task<OrderDto> ExecuteAsync(string orderNumber);
    }
}
