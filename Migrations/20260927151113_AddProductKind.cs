using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumen.Migrations
{
    /// <inheritdoc />
    public partial class AddProductKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Kind",
                schema: "dbo",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "ProductRecipes",
                schema: "dbo",
                columns: table => new
                {
                    ProductRecipeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentProductId = table.Column<long>(type: "bigint", nullable: false),
                    ComponentProductId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Condition = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRecipes", x => x.ProductRecipeId);
                    table.ForeignKey(
                        name: "FK_ProductRecipes_Products_ComponentProductId",
                        column: x => x.ComponentProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRecipes_Products_ParentProductId",
                        column: x => x.ParentProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Kind",
                schema: "dbo",
                table: "Products",
                column: "Kind");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRecipes_ComponentProductId",
                schema: "dbo",
                table: "ProductRecipes",
                column: "ComponentProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRecipes_ParentProductId",
                schema: "dbo",
                table: "ProductRecipes",
                column: "ParentProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductRecipes",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Products_Kind",
                schema: "dbo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Kind",
                schema: "dbo",
                table: "Products");
        }
    }
}
