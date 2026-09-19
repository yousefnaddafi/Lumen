namespace Lumen.Models.Enums
{
    public enum StockTransactionType
    {
        Purchase = 1,       // ورود از تامین‌کننده
        Sale = 2,           // خروج به‌خاطر فروش
        Return = 3,         // برگشت از مشتری
        Waste = 4,          // دور ریخته شده
        Adjustment = 5,     // اصلاح موجودی (انبارگردانی)
        TransferIn = 6,     // انتقال از شعبه دیگر
        TransferOut = 7,    // انتقال به شعبه دیگر
        InitialStock = 8    // موجودی اولیه
    }
}
