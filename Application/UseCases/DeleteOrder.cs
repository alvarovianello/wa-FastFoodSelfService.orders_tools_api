using Application.Interfaces.ExternalServices;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Domain.Enums;

namespace Application.UseCases
{
    public class DeleteOrder : IDeleteOrder
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentService _paymentService;

        public DeleteOrder(IOrderRepository orderRepository, IPaymentService paymentService)
        {
            _orderRepository = orderRepository;
            _paymentService = paymentService;
        }

        public async Task ExecuteAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception("Pedido não encontrado.");
            }

            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            if (payment != null && payment.Status != EnumStatusPayment.Pending.ToString())
            {
                throw new Exception("O pedido não pode ser excluído porque o pagamento já foi realizado.");
            }
            await _orderRepository.DeleteAsync(orderId);
        }
    }
}
