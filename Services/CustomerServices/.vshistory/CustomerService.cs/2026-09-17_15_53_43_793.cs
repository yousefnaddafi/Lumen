using Lumen.InfraStructure;
using Lumen.Models;
using Lumen.Services.UserServices;

namespace Lumen.Services.CustomerServices
{
    public class CustomerService : ICustomerService
    {
        private readonly IProjectEFRepository<Customer> _customerRepo;

        public CustomerService(IProjectEFRepository<Customer> customerRepo)
        {
            _customerRepo = customerRepo;
        }

        public List<Customer> GetAll(string? search = null)
        {
            var q = _customerRepo.GetQuery();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(c => c.FullName.Contains(s) || c.PhoneNumber.Contains(s));
            }
            return q.OrderByDescending(c => c.CustomerId).ToList();
        }

        public Customer? Get(long id) => _customerRepo.GetQuery().FirstOrDefault(c => c.CustomerId == id);

        public Customer? GetByPhone(string phone)
        {
            var p = (phone ?? "").Trim();
            if (string.IsNullOrEmpty(p)) return null;
            return _customerRepo.GetQuery().FirstOrDefault(c => c.PhoneNumber == p);
        }

        public Customer Create(Customer customer)
        {
            customer.CreatedAt = DateTime.Now;
            return _customerRepo.Insert(customer);
        }

        public Customer Update(Customer customer)
        {
            _customerRepo.VoidUpdate(customer);
            return customer;
        }

        public async Task<bool> Delete(long id)
        {
            var c = Get(id);
            if (c == null) return false;
            await _customerRepo.Delete(c);
            return true;
        }

        public Customer CreateOrGetByPhone(string phone, string fullName, long? branchId, bool isBusiness, string? userId)
        {
            phone = (phone ?? "").Trim();
            fullName = (fullName ?? "").Trim();

            var existing = GetByPhone(phone);
            if (existing != null)
            {
                if (!string.IsNullOrWhiteSpace(fullName) && existing.FullName != fullName)
                {
                    existing.FullName = fullName;
                    _customerRepo.VoidUpdate(existing);
                }
                return existing;
            }

            var customer = new Customer
            {
                FullName = fullName,
                PhoneNumber = phone,
                IsBusiness = isBusiness,
                RegisteredBranchId = branchId,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };
            return _customerRepo.Insert(customer);
        }
    }
}
