using Application.DTOs;

namespace Application.Interfaces.ExternalServices
{
    public interface ICustomerService
    {
        Task<CustomerDto?> GetCustomerByIdAsync(int customerId);
    }
}
