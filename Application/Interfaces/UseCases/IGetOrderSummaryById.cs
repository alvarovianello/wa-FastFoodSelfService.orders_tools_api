using Application.DTOs;

namespace Application.Interfaces.UseCases
{
    public interface IGetOrderSummaryById
    {
        Task<OrderSummaryDto?> ExecuteAsync(string orderNumber);
    }
}
