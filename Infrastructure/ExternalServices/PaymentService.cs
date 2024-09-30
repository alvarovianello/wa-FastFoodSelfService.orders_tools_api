using Application.DTOs;
using Application.Interfaces.ExternalServices;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Infrastructure.ExternalServices
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PaymentService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<PaymentStatusDto?> GetPaymentByOrderIdAsync(int orderId)
        {
            var paymentsApiUrl = _configuration["ExternalServices:PaymentsApiUrl"];
            var response = await _httpClient.GetAsync($"{paymentsApiUrl}/api/payments/order/{orderId}");

            if (!response.IsSuccessStatusCode)
            {
                // Lida com erro
                return null;
            }

            var paymentStatus = await response.Content.ReadFromJsonAsync<PaymentStatusDto>();
            return paymentStatus;
        }

        public async Task<IEnumerable<PaymentStatusDto?>> GetApprovedPaymentsAsync(int limit)
        {
            var paymentsApiUrl = _configuration["ExternalServices:PaymentsApiUrl"];
            var response = await _httpClient.GetAsync($"{paymentsApiUrl}/api/payment/approved?limit={limit}");

            if (!response.IsSuccessStatusCode)
            {
                // Lida com erro
                return Enumerable.Empty<PaymentStatusDto>();
            }

            var approvedPayments = await response.Content.ReadFromJsonAsync<IEnumerable<PaymentStatusDto>>();
            return approvedPayments ?? Enumerable.Empty<PaymentStatusDto>();
        }
    }
}
