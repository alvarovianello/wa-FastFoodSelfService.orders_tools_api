using Application.DTOs;

namespace Application.Interfaces.ExternalServices
{
    public interface IPaymentService
    {
        Task<PaymentStatusDto?> GetPaymentByOrderIdAsync(int orderId);
        Task<IEnumerable<PaymentStatusDto?>> GetApprovedPaymentsAsync(int limit);
    }
}
