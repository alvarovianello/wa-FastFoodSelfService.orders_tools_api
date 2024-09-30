using Application.DTOs;
using Application.Interfaces.ExternalServices;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Infrastructure.ExternalServices
{
    public class CustomerService : ICustomerService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CustomerService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int customerId)
        {
            var customersApiUrl = _configuration["ExternalServices:CustomersApiUrl"];
            var response = await _httpClient.GetAsync($"{customersApiUrl}/api/customer/{customerId}");

            if (!response.IsSuccessStatusCode)
            {
                // Lida com erro
                return null;
            }

            var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
            return customer;
        }
    }
}
