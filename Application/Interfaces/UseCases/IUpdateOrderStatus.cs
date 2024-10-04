using Domain.Enums;

namespace Application.Interfaces.UseCases
{
    public interface IUpdateOrderStatus
    {
        Task ExecuteAsync(int orderId, EnumOrderStatus newStatus);
    }
}
