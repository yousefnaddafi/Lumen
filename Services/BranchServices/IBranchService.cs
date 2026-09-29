using Lumen.Models;

namespace Lumen.Services.BranchServices
{
    public interface IBranchService
    {
        List<Branch> GetAll();
        Branch? Get(long id);
        Branch Create(Branch branch);
        Branch Update(Branch branch);
        Task<bool> Delete(long id);
        List<Branch> GetBranchesForUser(string userId);
        List<Branch> GetMyBranches();
    }
}
