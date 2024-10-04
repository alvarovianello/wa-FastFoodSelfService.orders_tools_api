using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Domain.Enums;

namespace Application.UseCases
{
    public class UpdateOrderStatus : IUpdateOrderStatus
    {
        private readonly IOrderRepository _orderRepository;

        public UpdateOrderStatus(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task ExecuteAsync(int orderId, EnumOrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Pedido não encontrado.");
            }
            if (!Enum.IsDefined(typeof(EnumOrderStatus), newStatus))
            {
                throw new Exception("Status informado inválido!");
            }

            if (order.Status == (int)EnumOrderStatus.Finalizado || order.Status == (int)EnumOrderStatus.Cancelado)
            {
                throw new Exception("Não é possível atualizar status de um pedido finalizado ou cancelado!");
            }

            order.Status = (int)newStatus;
            await _orderRepository.UpdateOrderStatusAsync(order);
        }
    }
}
