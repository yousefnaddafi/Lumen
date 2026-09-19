using Lumen.Models;

namespace Lumen.Services.CustomerServices
{
    public interface ICustomerService
    {
        List<Customer> GetAll(string? search = null);
        Customer? GetById(long id);
        Customer? GetByPhone(string phone);
        Customer CreateOrGetByPhone(string phone, string fullName, long? branchId, bool isBusiness, string? userId);
        Customer Update(Customer customer);
        void Delete(long id);
    }
}
