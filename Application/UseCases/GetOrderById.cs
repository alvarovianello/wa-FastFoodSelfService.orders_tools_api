using Application.DTOs;
using Application.Interfaces.ExternalServices;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Domain.Enums;

namespace Application.UseCases
{
    public class GetOrderById : IGetOrderById
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentService _paymentService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        public GetOrderById(IOrderRepository orderRepository, IPaymentService paymentService, ICustomerService customerService, IProductService productService)
        {
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            _customerService = customerService;
            _productService = productService;
        }

        public async Task<OrderDto?> ExecuteAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                return null;

            var payment = await _paymentService.GetPaymentByOrderIdAsync(order.Id);
            var paymentStatus = payment != null ? payment.Status.ToString() : EnumStatusPayment.Pending.ToString();

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            
            var productIds = order.OrderItems.Select(item => item.ProductId).ToArray();
            var products = await _productService.GetAllProductsAsync(productIds);

            if (!products.Any())
                throw new Exception("Nenhum produto encontrado.");

            return new OrderDto(order, paymentStatus, products, customer);
        }
    }
}
