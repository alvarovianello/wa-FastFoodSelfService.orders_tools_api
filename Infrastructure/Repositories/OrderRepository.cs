using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IConfiguration _configuration;

        public OrderRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            var query = @"SELECT o.id, o.customer_id as CustomerId, o.order_number as OrderNumber, o.total_price as TotalPrice, o.created_at as CreatedAt, o.status AS Status
                      FROM dbo.Orders o 
                      WHERE o.id = @Id";

            using (var connection = CreateConnection())
            {
                var order = await connection.QueryFirstOrDefaultAsync<Order>(query, new { Id = id });
                if (order != null)
                {
                    order.OrderItems = (await GetOrderItemsAsync(order.Id)).ToList();
                }
                return order;
            }
        }

        public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
        {
            var query = @"SELECT o.id, o.customer_id as CustomerId, o.order_number as OrderNumber, o.total_price as TotalPrice, o.created_at as CreatedAt, o.status AS Status
                      FROM dbo.Orders o 
                      WHERE o.order_number = @OrderNumber";

            using (var connection = CreateConnection())
            {
                var order = await connection.QueryFirstOrDefaultAsync<Order>(query, new { OrderNumber = orderNumber });
                if (order != null)
                {
                    order.OrderItems = (await GetOrderItemsAsync(order.Id)).ToList();
                }
                return order;
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var query = @"SELECT o.id, o.customer_id as CustomerId, o.order_number as OrderNumber, o.total_price as TotalPrice, o.created_at as CreatedAt, o.status AS Status
                      FROM dbo.Orders o ";

            using (var connection = CreateConnection())
            {
                var orders = await connection.QueryAsync<Order>(query);
                foreach (var order in orders)
                {
                    order.OrderItems = (await GetOrderItemsAsync(order.Id)).ToList();
                }
                return orders;
            }
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(EnumOrderStatus status)
        {
            var query = @"SELECT o.id, o.customer_id as CustomerId, o.order_number as OrderNumber, o.total_price as TotalPrice, o.created_at as CreatedAt, o.status AS Status
                      FROM dbo.Orders o
                      WHERE o.status = @Status 
                      ORDER BY created_at DESC";

            using (var connection = CreateConnection())
            {
                var orders = await connection.QueryAsync<Order>(query, new { Status = (int)status });
                foreach (var order in orders)
                {
                    order.OrderItems = (await GetOrderItemsAsync(order.Id)).ToList();
                }
                return orders;
            }
        }

        public async Task AddAsync(Order order)
        {
            var queryOrder = "INSERT INTO dbo.Orders (customer_id, order_number, total_price, status) " +
                             "VALUES (@CustomerId, @OrderNumber, @TotalPrice, @Status) RETURNING id";

            var queryOrderItem = "INSERT INTO dbo.OrderItem (order_id, product_id, quantity, total_price) " +
                                 "VALUES (@OrderId, @ProductId, @Quantity, @TotalPrice)";

            var queryOrderStatus = "INSERT INTO dbo.OrderStatus (order_id, status) " +
                                   "VALUES (@OrderId, @Status)";

            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    var orderId = await connection.ExecuteScalarAsync<int>(queryOrder, new
                    {
                        order.CustomerId,
                        order.OrderNumber,
                        order.TotalPrice,
                        order.Status
                    }, transaction);

                    foreach (var item in order.OrderItems)
                    {
                        await connection.ExecuteAsync(queryOrderItem, new
                        {
                            OrderId = orderId,
                            item.ProductId,
                            item.Quantity,
                            item.TotalPrice
                        }, transaction);
                    }

                    await connection.ExecuteAsync(queryOrderStatus, new
                    {
                        OrderId = orderId,
                        Status = order.Status
                    }, transaction);

                    await transaction.CommitAsync();
                }
            }
        }

        public async Task UpdateOrderAsync(Order order)
        {
            var updateOrderQuery = "UPDATE dbo.Orders SET customer_id = @CustomerId, total_price = @TotalPrice WHERE id = @Id";
            var deleteOrderItemsQuery = "DELETE FROM dbo.OrderItem WHERE order_id = @OrderId";
            var insertOrderItemsQuery = "INSERT INTO dbo.OrderItem (order_id, product_id, quantity, total_price) VALUES (@OrderId, @ProductId, @Quantity, @TotalPrice)";

            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(updateOrderQuery, new { order.CustomerId, order.TotalPrice, order.Id });

                await connection.ExecuteAsync(deleteOrderItemsQuery, new { OrderId = order.Id });

                foreach (var item in order.OrderItems)
                {
                    await connection.ExecuteAsync(insertOrderItemsQuery, new
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice
                    });
                }
            }
        }

        public async Task UpdateOrderStatusAsync(Order order)
        {
            var updateQuery = "UPDATE dbo.Orders SET status = @Status WHERE id = @Id";
            var insertQuery = "INSERT INTO dbo.OrderStatus (order_id, status) VALUES (@OrderId, @Status)";

            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(updateQuery, new { order.Status, order.Id });

                await connection.ExecuteAsync(insertQuery, new { OrderId = order.Id, Status = order.Status });
            }
        }

        public async Task DeleteAsync(int id)
        {
            var queryOrder = "DELETE FROM dbo.Orders WHERE id = @Id";
            var queryOrderItem = "DELETE FROM dbo.OrderItem WHERE order_id = @Id";
            var queryOrderStatus = "DELETE FROM dbo.OrderStatus WHERE order_id = @Id";

            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    await connection.ExecuteAsync(queryOrderItem, new { Id = id }, transaction);
                    await connection.ExecuteAsync(queryOrderStatus, new { Id = id }, transaction);
                    await connection.ExecuteAsync(queryOrder, new { Id = id }, transaction);

                    await transaction.CommitAsync();
                }
            }
        }

        private async Task<IEnumerable<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            var query = @"SELECT o.*, o.product_id as ProductId, o.order_id as OrderId, o.total_price as TotalPrice
                          FROM dbo.OrderItem o
                          WHERE o.order_id = @OrderId";

            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<OrderItem>(query, new { OrderId = orderId });
            }
        }
    }
}
