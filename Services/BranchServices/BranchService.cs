using Lumen.InfraStructure;
using Lumen.Models;

namespace Lumen.Services.BranchServices
{
    public class BranchService : IBranchService
    {
        private readonly IProjectEFRepository<Branch> _repo;
        public BranchService(IProjectEFRepository<Branch> repo) => _repo = repo;

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
