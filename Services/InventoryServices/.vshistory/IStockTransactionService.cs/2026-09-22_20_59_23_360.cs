using Lumen.Models;
using Lumen.Models.Enums;

namespace Lumen.Services.InventoryServices
{
    public interface IStockTransactionService
    {
        void Adjust(AdjustStockDto dto);
        List<StockTransactionRowDto> GetHistory(long? branchId = null, long? productId = null,
                                                  DateTime? from = null, DateTime? to = null,
                                                  StockTransactionType? type = null);
    }
}
