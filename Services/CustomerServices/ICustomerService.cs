using Lumen.Models;

namespace Lumen.Services.CustomerServices
{
    public interface ICustomerService
    {
        List<Customer> GetAll(string? search = null);
        Customer? Get(long id);
        Customer? GetByPhone(string phone);
        Customer Create(Customer customer);
        Customer Update(Customer customer);
        Task<bool> Delete(long id);
        Customer CreateOrGetByPhone(string phone, string fullName, long? branchId, bool isBusiness);
    }
}
