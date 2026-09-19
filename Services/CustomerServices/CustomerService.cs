using Lumen.InfraStructure;
using Lumen.Models;
using Lumen.Services.UserServices;

namespace Lumen.Services.CustomerServices
{
    public class CustomerService : ICustomerService
    {
        private readonly IProjectEFRepository<Customer> _repo;
        public CustomerService(IProjectEFRepository<Customer> repo) => _repo = repo;

        public List<Customer> GetAll(string? search = null)
        {
            var q = _repo.GetQuery();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(c => c.FullName.Contains(s) || c.PhoneNumber.Contains(s));
            }
            return q.OrderByDescending(c => c.CustomerId).ToList();
        }

        public Customer? Get(long id) =>
            _repo.GetQuery().FirstOrDefault(c => c.CustomerId == id);

        public Customer? GetByPhone(string phone)
        {
            var p = (phone ?? "").Trim();
            if (string.IsNullOrEmpty(p)) return null;
            return _repo.GetQuery().FirstOrDefault(c => c.PhoneNumber == p);
        }

        public Customer Create(Customer customer) => _repo.Insert(customer);

        public Customer Update(Customer customer)
        {
            _repo.VoidUpdate(customer);
            return customer;
        }

        public async Task<bool> Delete(long id)
        {
            var c = Get(id);
            if (c == null) return false;
            await _repo.Delete(c);
            return true;
        }

        public Customer CreateOrGetByPhone(string phone, string fullName, long? branchId, bool isBusiness)
        {
            phone = (phone ?? "").Trim();
            fullName = (fullName ?? "").Trim();

            var existing = GetByPhone(phone);
            if (existing != null)
            {
                if (!string.IsNullOrWhiteSpace(fullName) && existing.FullName != fullName)
                {
                    existing.FullName = fullName;
                    _repo.VoidUpdate(existing);
                }
                return existing;
            }

            return _repo.Insert(new Customer
            {
                FullName = fullName,
                PhoneNumber = phone,
                IsBusiness = isBusiness,
                RegisteredBranchId = branchId,
                IsDeleted = false
            });
        }
    }
}
