using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumen.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "dbo",
                columns: table => new
                {
                    CustomerId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBusiness = table.Column<bool>(type: "bit", nullable: false),
                    RegisteredBranchId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customers_Branches_RegisteredBranchId",
                        column: x => x.RegisteredBranchId,
                        principalSchema: "dbo",
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                schema: "dbo",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_FullName",
                schema: "dbo",
                table: "Customers",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PhoneNumber",
                schema: "dbo",
                table: "Customers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_RegisteredBranchId",
                schema: "dbo",
                table: "Customers",
                column: "RegisteredBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                schema: "dbo",
                table: "Orders",
                column: "CustomerId",
                principalSchema: "dbo",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                schema: "dbo",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "Customers",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId",
                schema: "dbo",
                table: "Orders");
        }
    }
}
