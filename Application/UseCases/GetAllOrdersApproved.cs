using Application.DTOs;
using Application.Interfaces.ExternalServices;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;

namespace Application.UseCases
{
    public class GetAllOrdersApproved : IGetAllOrdersApproved
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentService _paymentService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        public GetAllOrdersApproved(IOrderRepository orderRepository, IPaymentService paymentService, ICustomerService customerService, IProductService productService)
        {
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            _customerService = customerService;
            _productService = productService;
        }

        public async Task<IEnumerable<OrderDto>> ExecuteAsync(int limit = 20)
        {
            var approvedPayments = await _paymentService.GetApprovedPaymentsAsync(limit);

            var orderResponses = new List<OrderDto>();
            foreach (var payment in approvedPayments)
            {
                var order = await _orderRepository.GetByIdAsync(payment.OrderId);

                if (order == null)
                    continue;

                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

                var productIds = order.OrderItems.Select(item => item.ProductId).ToArray();
                var products = await _productService.GetAllProductsAsync(productIds);

                if (!products.Any())
                    throw new Exception("Nenhum produto encontrado.");

                var response = new OrderDto(order, payment.Status.ToString(), products, customer);
                orderResponses.Add(response);
            }

            return orderResponses.OrderBy(o => o.CreatedAt);
        }
    }
}
