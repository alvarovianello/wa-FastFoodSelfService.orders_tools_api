using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByOrderNumberAsync(string orderNumber);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetByStatusAsync(EnumOrderStatus status);
        Task AddAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task UpdateOrderStatusAsync(Order order);
        Task DeleteAsync(int id);
    }
}
