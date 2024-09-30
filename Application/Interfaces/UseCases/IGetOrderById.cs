using Application.DTOs;

namespace Application.Interfaces.UseCases
{
    public interface IGetOrderById
    {
        Task<OrderDto> ExecuteAsync(int id);
    }
}
