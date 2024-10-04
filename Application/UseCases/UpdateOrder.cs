using Application.DTOs;
using Application.Interfaces.ExternalServices;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases
{
    public class UpdateOrder : IUpdateOrder
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentService _paymentService;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;

        public UpdateOrder(IOrderRepository orderRepository, ICustomerService customerService, IPaymentService paymentService, IProductService productService)
        {
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            _productService = productService;
            _customerService = customerService;
        }

        public async Task<OrderDto?> ExecuteAsync(UpdateOrderRequest request)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null || order.Status != (int)EnumOrderStatus.Recebido)
            {
                throw new Exception("O pedido não pode ser atualizado porque não está no status 'Recebido'.");
            }

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            if (customer == null)
            {
                throw new Exception("Cliente informado não encontrado.");
            }

            var payment = await _paymentService.GetPaymentByOrderIdAsync(order.Id);
            if (payment != null)
            {
                throw new Exception("O pedido não pode ser atualizado pois já foi gerado o QR Code de pagamento");
            }

            var products = await _productService.GetAllProductsAsync(request.OrderItems.Select(item => item.ProductId).ToArray());
            if (!products.Any())
            {
                throw new Exception("Nenhum produto encontrado.");
            }

            decimal totalPrice = 0;
            order.OrderItems.Clear();

            foreach (var item in request.OrderItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null)
                {
                    totalPrice += product.Price * item.Quantity;
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        TotalPrice = product.Price
                    });
                }
            }

            order.TotalPrice = totalPrice;

            await _orderRepository.UpdateOrderAsync(order);

            return new OrderDto(order, EnumStatusPayment.Pending.ToString(), products, customer);
        }
    }
}
