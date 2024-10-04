using Application.DTOs;
using Application.Interfaces.ExternalServices;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;

namespace Application.UseCases
{
    public class GetOrderSummaryById : IGetOrderSummaryById
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;

        public GetOrderSummaryById(IOrderRepository orderRepository, IProductService productService)
        {
            _orderRepository = orderRepository;
            _productService = productService;
        }

        public async Task<OrderSummaryDto?> ExecuteAsync(string orderNumber)
        {
            var order = await _orderRepository.GetByOrderNumberAsync(orderNumber);

            if (order == null)
                return null;

            var productIds = order.OrderItems.Select(item => item.ProductId).ToArray();
            var products = await _productService.GetAllProductsAsync(productIds);

            if (!products.Any())
                throw new Exception("Nenhum produto encontrado.");

            return new OrderSummaryDto(order, products);
        }
    }
}
