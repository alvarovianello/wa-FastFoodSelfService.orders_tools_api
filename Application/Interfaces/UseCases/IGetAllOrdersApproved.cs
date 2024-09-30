using Application.DTOs;

namespace Application.Interfaces.UseCases
{
    public interface IGetAllOrdersApproved
    {
        Task<IEnumerable<OrderDto>> ExecuteAsync(int limit);
    }
}
