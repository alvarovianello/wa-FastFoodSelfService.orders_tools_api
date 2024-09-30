using Application.DTOs;

namespace Application.Interfaces.UseCases
{
    public interface IGetAllOrders
    {
        Task<IEnumerable<OrderDto>> ExecuteAsync();
    }
}
