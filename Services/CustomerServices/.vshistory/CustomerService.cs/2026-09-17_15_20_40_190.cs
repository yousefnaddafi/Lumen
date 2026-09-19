using Lumen.InfraStructure;
using Lumen.Models;
using Lumen.Services.UserServices;

namespace Lumen.Services.CustomerServices
{
    public class CustomerService : ICustomerService
    {
        private readonly ProjectDBContext _db;
        private readonly ICurrentUserService _currentUser;

        public CustomerService(ProjectDBContext db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }

        public List<Customer> GetAll(string? search = null)
        {
            var q = _db.Customers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(c => c.FullName.Contains(s) || c.PhoneNumber.Contains(s));
            }
            return q.OrderByDescending(c => c.CustomerId).ToList();
        }

        public Customer? GetById(long id) => _db.Customers.FirstOrDefault(c => c.CustomerId == id);

        public Customer? GetByPhone(string phone) =>
            _db.Customers.FirstOrDefault(c => c.PhoneNumber == phone);

        public Customer CreateOrGetByPhone(string phone, string fullName, long? branchId, bool isBusiness, string? userId)
        {
            phone = (phone ?? "").Trim();
            fullName = (fullName ?? "").Trim();

            var existing = GetByPhone(phone);
            if (existing != null)
            {
                // Refresh name if the caller provided a newer one
                if (!string.IsNullOrWhiteSpace(fullName) && existing.FullName != fullName)
                {
                    existing.FullName = fullName;
                    _db.SaveChanges();
                }
                return existing;
            }

            var customer = new Customer
            {
                FullName = fullName,
                PhoneNumber = phone,
                IsBusiness = isBusiness,
                RegisteredBranchId = branchId,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId
            };
            _db.Customers.Add(customer);
            _db.SaveChanges();
            return customer;
        }

        public Customer Update(Customer customer)
        {
            _db.Customers.Update(customer);
            _db.SaveChanges();
            return customer;
        }

        public void Delete(long id)
        {
            var c = _db.Customers.FirstOrDefault(x => x.CustomerId == id);
            if (c != null)
            {
                c.IsDeleted = true;
                _db.SaveChanges();
            }
        }
    }
}
