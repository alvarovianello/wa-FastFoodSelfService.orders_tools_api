using Application.Interfaces.ExternalServices;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Application.UseCases;
using Infrastructure.ExternalServices;
using Infrastructure.Repositories;

namespace Api.Extensions
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddResolveDependencies(this WebApplicationBuilder builder)
        {
            IServiceCollection services = builder.Services;

            //External Services
            services.AddHttpClient<ICustomerService, CustomerService>();
            services.AddHttpClient<IProductService, ProductService>();
            services.AddHttpClient<IPaymentService, PaymentService>();

            //Order
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICreateOrder, CreateOrder>();
            services.AddScoped<IDeleteOrder, DeleteOrder>();
            services.AddScoped<IGetAllOrders, GetAllOrders>();
            services.AddScoped<IGetAllOrdersApproved, GetAllOrdersApproved>();
            services.AddScoped<IGetOrderById, GetOrderById>();
            services.AddScoped<IGetOrderSummaryById, GetOrderSummaryById>();
            services.AddScoped<IGetOrderByOrderNumber, GetOrderByOrderNumber>();
            services.AddScoped<IGetOrderByStatus, GetOrderByStatus>();
            services.AddScoped<IUpdateOrder, UpdateOrder>();
            services.AddScoped<IUpdateOrderStatus, UpdateOrderStatus>();


            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddHttpClient();

            return services;
        }
    }
}
