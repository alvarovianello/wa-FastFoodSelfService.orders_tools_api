using Application.DTOs;
using Application.Interfaces.ExternalServices;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Domain.Enums;

namespace Application.UseCases
{
    public class GetAllOrders : IGetAllOrders
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentService _paymentService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        public GetAllOrders(IOrderRepository orderRepository, IPaymentService paymentService, ICustomerService customerService, IProductService productService)
        {
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            _customerService = customerService;
            _productService = productService;
        }

        public async Task<IEnumerable<OrderDto>> ExecuteAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            var orderResponses = new List<OrderDto>();

            foreach (var order in orders)
            {
                var payment = await _paymentService.GetPaymentByOrderIdAsync(order.Id);
                var paymentStatus = payment != null ? payment.Status.ToString() : EnumStatusPayment.Pending.ToString();

                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

                var productIds = order.OrderItems.Select(item => item.ProductId).ToArray();
                var products = await _productService.GetAllProductsAsync(productIds);

                if (!products.Any())
                {
                    throw new Exception("Nenhum produto encontrado.");
                }
                var response = new OrderDto(order, paymentStatus, products, customer);
                orderResponses.Add(response);
            }

            return orderResponses;
        }
    }
}
