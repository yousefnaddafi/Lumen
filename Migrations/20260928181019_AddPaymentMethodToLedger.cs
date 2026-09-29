using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumen.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodToLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                schema: "dbo",
                table: "CashLedgerEntries",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                schema: "dbo",
                table: "CashLedgerEntries");
        }
    }
}
