using Lumen.Models;

namespace Lumen.Services.InventoryServices
{
    public interface IInventoryService
    {
        List<InventoryRowDto> GetInventory(long? branchId = null, string? search = null, bool onlyLow = false);
        InventoryRowDto? GetOne(long branchId, long productId);
        void SetLevels(SetInventoryLevelDto dto);
        void EnsureInventoryRowsForBranch(long branchId);
    }
}
