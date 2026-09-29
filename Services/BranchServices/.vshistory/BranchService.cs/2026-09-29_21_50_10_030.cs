using Lumen.InfraStructure;
using Lumen.Models;
using Lumen.Services.UserServices;

namespace Lumen.Services.BranchServices
{
    public class BranchService : IBranchService
    {
        private readonly IProjectEFRepository<UserBranch> _userBranchRepo;
        private readonly IProjectEFRepository<Branch> _branchRepo;  
        private readonly ICurrentUserService _currentUser;
        public BranchService(IProjectEFRepository<Branch> branchRepo,
            IProjectEFRepository<UserBranch> userBranchRepo, ICurrentUserService currentUser) 
        {
            _currentUser = currentUser;
            _branchRepo = branchRepo;
            _userBranchRepo = userBranchRepo;
        }

        public List<Branch> GetAll() =>
            _repo.GetQuery().OrderBy(b => b.Title).ToList();

        public Branch? Get(long id) =>
            _repo.GetQuery().FirstOrDefault(b => b.BranchId == id);

        public Branch Create(Branch branch)
        {
            branch.CreateDate = DateTime.Now;
            return _repo.Insert(branch);
        }

        public Branch Update(Branch branch)
        {
            _repo.VoidUpdate(branch);
            return branch;
        }

        public async Task<bool> Delete(long id)
        {
            var b = Get(id);
            if (b == null) return false;
            await _repo.Delete(b);
            return true;
        }
    }
}
