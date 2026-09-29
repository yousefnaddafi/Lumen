using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumen.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchProductAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BranchProductAvailabilities",
                schema: "dbo",
                columns: table => new
                {
                    BranchProductAvailabilityId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchProductAvailabilities", x => x.BranchProductAvailabilityId);
                    table.ForeignKey(
                        name: "FK_BranchProductAvailabilities_Branches_BranchId",
                        column: x => x.BranchId,
                        principalSchema: "dbo",
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BranchProductAvailabilities_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchProductAvailabilities_BranchId_IsAvailable",
                schema: "dbo",
                table: "BranchProductAvailabilities",
                columns: new[] { "BranchId", "IsAvailable" });

            migrationBuilder.CreateIndex(
                name: "IX_BranchProductAvailabilities_BranchId_ProductId",
                schema: "dbo",
                table: "BranchProductAvailabilities",
                columns: new[] { "BranchId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BranchProductAvailabilities_ProductId",
                schema: "dbo",
                table: "BranchProductAvailabilities",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BranchProductAvailabilities",
                schema: "dbo");
        }
    }
}
