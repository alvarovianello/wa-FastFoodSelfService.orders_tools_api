using Application.DTOs;

namespace Application.Interfaces.ExternalServices
{
    public interface IProductService
    {
        Task<ProductDto?> GetProductByIdAsync(int productId);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync(int[]? productIds = null);
    }
}
