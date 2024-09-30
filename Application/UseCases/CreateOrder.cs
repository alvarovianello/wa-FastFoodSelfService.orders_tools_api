using Application.DTOs;
using Application.Interfaces.ExternalServices;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases
{
    public class CreateOrder : ICreateOrder
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;

        public CreateOrder(IOrderRepository orderRepository, ICustomerService customerService, IProductService productService)
        {
            _orderRepository = orderRepository;
            _customerService = customerService;
            _productService = productService;
        }

        public async Task<OrderDto> ExecuteAsync(CreateOrderRequest request)
        {
            // Verifica se o cliente existe
            var customer = await _customerService.GetCustomerByIdAsync(request.CustomerId);
            if (customer == null)
            {
                throw new Exception("Cliente informado não encontrado.");
            }

            // Busca os produtos
            var products = await _productService.GetAllProductsAsync(request.OrderItems.Select(item => item.ProductId).ToArray());
            if (!products.Any())
            {
                throw new Exception("Nenhum produto encontrado.");
            }

            decimal totalPriceOrder = 0;
            var orderItems = new List<OrderItem>();
            foreach (var item in request.OrderItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null)
                {
                    totalPriceOrder += product.Price * item.Quantity;
                    orderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        TotalPrice = product.Price * item.Quantity
                    });
                }
            }

            var order = new Order
            {
                CustomerId = request.CustomerId,
                OrderNumber = GenerateOrderNumber(),
                TotalPrice = totalPriceOrder,
                Status = (int)EnumOrderStatus.Recebido,
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            await _orderRepository.AddAsync(order);

            return new OrderDto(order, EnumStatusPayment.Pending.ToString(), products, customer);
        }

        private string GenerateOrderNumber()
        {
            return new Random().Next(10000, 99999).ToString();
        }
    }
}
