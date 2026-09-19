using Lumen.Models;

namespace Lumen.Services.OrderServices
{
    public interface IOrderService
    {
        (Order order, Customer? customer, bool customerCreated) SubmitOrder(SubmitOrderDto dto);
        List<Order> GetByBranch(long branchId, DateTime? from = null, DateTime? to = null);
        Order? Get(long id);
    }
}
