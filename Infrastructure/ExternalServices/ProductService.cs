using Application.DTOs;
using Application.Interfaces.ExternalServices;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Infrastructure.ExternalServices
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ProductService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ProductDto?> GetProductByIdAsync(int productId)
        {
            var productsApiUrl = _configuration["ExternalServices:ProductsApiUrl"];
            var response = await _httpClient.GetAsync($"{productsApiUrl}/api/product/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                // Lida com erro
                return null;
            }

            var product = await response.Content.ReadFromJsonAsync<ProductDto>();
            return product;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(int[]? productIds = null)
        {
            var productsApiUrl = _configuration["ExternalServices:ProductsApiUrl"];
            string requestUri;

            if (productIds != null && productIds.Length > 0)
            {
                var idsQuery = string.Join(",", productIds);
                requestUri = $"{productsApiUrl}/api/product/all?productIds={idsQuery}";
            }
            else
            {
                requestUri = $"{productsApiUrl}/api/product/all";
            }

            var response = await _httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                // Lida com erro
                return Enumerable.Empty<ProductDto>(); // Retorna uma lista vazia em caso de erro
            }

            var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
            return products;
        }
    }
}
