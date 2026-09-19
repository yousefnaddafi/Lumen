using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lumen.Migrations
{
    /// <inheritdoc />
    public partial class InitialFullSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                schema: "dbo",
                columns: table => new
                {
                    BranchId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.BranchId);
                });

            migrationBuilder.CreateTable(
                name: "IdentityRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LedgerCategories",
                schema: "dbo",
                columns: table => new
                {
                    LedgerCategoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerCategories", x => x.LedgerCategoryId);
                    table.ForeignKey(
                        name: "FK_LedgerCategories_LedgerCategories_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "dbo",
                        principalTable: "LedgerCategories",
                        principalColumn: "LedgerCategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            //migrationBuilder.CreateTable(
            //    name: "Medias",
            //    schema: "dbo",
            //    columns: table => new
            //    {
            //        MediaId = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ObjectId = table.Column<long>(type: "bigint", nullable: false),
            //        PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Type = table.Column<int>(type: "int", nullable: false),
            //        Priority = table.Column<int>(type: "int", nullable: true),
            //        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
            //        UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Medias", x => x.MediaId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProductCategories",
            //    schema: "dbo",
            //    columns: table => new
            //    {
            //        ProductCategoryId = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProductCategories", x => x.ProductCategoryId);
            //    });

            migrationBuilder.CreateTable(
                name: "UserActionLogs",
                schema: "dbo",
                columns: table => new
                {
                    UserActionLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Details = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActionLogs", x => x.UserActionLogId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "dbo",
                columns: table => new
                {
                    OrderId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Channel = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CustomerPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CustomerId = table.Column<long>(type: "bigint", nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CostTotal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Branches_BranchId",
                        column: x => x.BranchId,
                        principalSchema: "dbo",
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserBranches",
                schema: "dbo",
                columns: table => new
                {
                    UserBranchId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBranches", x => x.UserBranchId);
                    table.ForeignKey(
                        name: "FK_UserBranches_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBranches_Branches_BranchId",
                        column: x => x.BranchId,
                        principalSchema: "dbo",
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                });

            //migrationBuilder.CreateTable(
            //    name: "Products",
            //    schema: "dbo",
            //    columns: table => new
            //    {
            //        ProductId = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            //        IsAvailable = table.Column<bool>(type: "bit", nullable: false),
            //        ProductCategoryId = table.Column<long>(type: "bigint", nullable: false),
            //        CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
            //        EcoPrice = table.Column<long>(type: "bigint", nullable: false),
            //        BusinessPrice = table.Column<long>(type: "bigint", nullable: true),
            //        Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
            //        CurrentUnitCost = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
            //        Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
            //        TracksInventory = table.Column<bool>(type: "bit", nullable: false),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Products", x => x.ProductId);
            //        table.ForeignKey(
            //            name: "FK_Products_ProductCategories_ProductCategoryId",
            //            column: x => x.ProductCategoryId,
            //            principalSchema: "dbo",
            //            principalTable: "ProductCategories",
            //            principalColumn: "ProductCategoryId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "dbo",
                columns: table => new
                {
                    PaymentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "dbo",
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchInventories",
                schema: "dbo",
                columns: table => new
                {
                    BranchInventoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    QuantityOnHand = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ReorderLevel = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    MaxStock = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchInventories", x => x.BranchInventoryId);
                    table.ForeignKey(
                        name: "FK_BranchInventories_Branches_BranchId",
                        column: x => x.BranchId,
                        principalSchema: "dbo",
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BranchInventories_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTransfers",
                schema: "dbo",
                columns: table => new
                {
                    InventoryTransferId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromBranchId = table.Column<long>(type: "bigint", nullable: false),
                    ToBranchId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OutTransactionId = table.Column<long>(type: "bigint", nullable: true),
                    InTransactionId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTransfers", x => x.InventoryTransferId);
                    table.ForeignKey(
                        name: "FK_InventoryTransfers_Branches_FromBranchId",
                        column: x => x.FromBranchId,
                        principalSchema: "dbo",
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryTransfers_Branches_ToBranchId",
                        column: x => x.ToBranchId,
                        principalSchema: "dbo",
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryTransfers_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                schema: "dbo",
                columns: table => new
                {
                    OrderItemId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ProductTitleSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UnitCostSnapshot = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    LineCost = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "dbo",
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockTransactions",
                schema: "dbo",
                columns: table => new
                {
                    StockTransactionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrderId = table.Column<long>(type: "bigint", nullable: true),
                    RelatedTransactionId = table.Column<long>(type: "bigint", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransactions", x => x.StockTransactionId);
                    table.ForeignKey(
                        name: "FK_StockTransactions_Branches_BranchId",
                        column: x => x.BranchId,
                        principalSchema: "dbo",
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTransactions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "dbo",
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StockTransactions_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "dbo",
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CashLedgerEntries",
                schema: "dbo",
                columns: table => new
                {
                    CashLedgerEntryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<long>(type: "bigint", nullable: false),
                    LedgerCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OrderId = table.Column<long>(type: "bigint", nullable: true),
                    StockTransactionId = table.Column<long>(type: "bigint", nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashLedgerEntries", x => x.CashLedgerEntryId);
                    table.ForeignKey(
                        name: "FK_CashLedgerEntries_Branches_BranchId",
                        column: x => x.BranchId,
                        principalSchema: "dbo",
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashLedgerEntries_LedgerCategories_LedgerCategoryId",
                        column: x => x.LedgerCategoryId,
                        principalSchema: "dbo",
                        principalTable: "LedgerCategories",
                        principalColumn: "LedgerCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CashLedgerEntries_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "dbo",
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CashLedgerEntries_StockTransactions_StockTransactionId",
                        column: x => x.StockTransactionId,
                        principalSchema: "dbo",
                        principalTable: "StockTransactions",
                        principalColumn: "StockTransactionId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "role-account", null, "Accountant", "ACCOUNTANT" },
                    { "role-barista", null, "Barista", "BARISTA" },
                    { "role-cashier", null, "Cashier", "CASHIER" },
                    { "role-manager", null, "BranchManager", "BRANCHMANAGER" },
                    { "role-menu", null, "MenuAdmin", "MENUADMIN" },
                    { "role-owner", null, "Owner", "OWNER" }
                });

            migrationBuilder.InsertData(
                table: "IdentityRole",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "role-account", null, "Accountant", "ACCOUNTANT" },
                    { "role-barista", null, "Barista", "BARISTA" },
                    { "role-cashier", null, "Cashier", "CASHIER" },
                    { "role-manager", null, "BranchManager", "BRANCHMANAGER" },
                    { "role-menu", null, "MenuAdmin", "MENUADMIN" },
                    { "role-owner", null, "Owner", "OWNER" }
                });

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "LedgerCategories",
                columns: new[] { "LedgerCategoryId", "Code", "Description", "Direction", "IsActive", "IsDeleted", "IsSystem", "ParentId", "SortOrder", "Title" },
                values: new object[,]
                {
                    { 1L, "SALE", null, 1, true, false, true, null, 1, "فروش" },
                    { 2L, "REFUND", null, 2, true, false, true, null, 2, "استرداد" },
                    { 3L, "PUR-ING", null, 2, true, false, false, null, 10, "خرید مواد اولیه" },
                    { 4L, "SALARY", null, 2, true, false, false, null, 11, "حقوق پرسنل" },
                    { 5L, "RENT", null, 2, true, false, false, null, 12, "اجاره" },
                    { 6L, "UTIL", null, 2, true, false, false, null, 13, "قبوض" },
                    { 7L, "EQUIP", null, 2, true, false, false, null, 14, "تجهیزات" },
                    { 8L, "MKT", null, 2, true, false, false, null, 15, "بازاریابی" },
                    { 9L, "OTHER", null, 3, true, false, false, null, 99, "سایر" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_Title",
                schema: "dbo",
                table: "Branches",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_BranchInventories_BranchId_ProductId",
                schema: "dbo",
                table: "BranchInventories",
                columns: new[] { "BranchId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BranchInventories_BranchId_ProductId_LastUpdated",
                schema: "dbo",
                table: "BranchInventories",
                columns: new[] { "BranchId", "ProductId", "LastUpdated" });

            migrationBuilder.CreateIndex(
                name: "IX_BranchInventories_ProductId",
                schema: "dbo",
                table: "BranchInventories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CashLedgerEntries_BranchId_LedgerCategoryId_OccurredAt",
                schema: "dbo",
                table: "CashLedgerEntries",
                columns: new[] { "BranchId", "LedgerCategoryId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CashLedgerEntries_BranchId_OccurredAt",
                schema: "dbo",
                table: "CashLedgerEntries",
                columns: new[] { "BranchId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CashLedgerEntries_LedgerCategoryId",
                schema: "dbo",
                table: "CashLedgerEntries",
                column: "LedgerCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CashLedgerEntries_OrderId",
                schema: "dbo",
                table: "CashLedgerEntries",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CashLedgerEntries_StockTransactionId",
                schema: "dbo",
                table: "CashLedgerEntries",
                column: "StockTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransfers_FromBranchId_CreatedAt",
                schema: "dbo",
                table: "InventoryTransfers",
                columns: new[] { "FromBranchId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransfers_ProductId",
                schema: "dbo",
                table: "InventoryTransfers",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransfers_ToBranchId_CreatedAt",
                schema: "dbo",
                table: "InventoryTransfers",
                columns: new[] { "ToBranchId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_LedgerCategories_Code",
                schema: "dbo",
                table: "LedgerCategories",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerCategories_ParentId",
                schema: "dbo",
                table: "LedgerCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerCategories_Title",
                schema: "dbo",
                table: "LedgerCategories",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                schema: "dbo",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                schema: "dbo",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BranchId_CreatedAt",
                schema: "dbo",
                table: "Orders",
                columns: new[] { "BranchId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BranchId_Status",
                schema: "dbo",
                table: "Orders",
                columns: new[] { "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                schema: "dbo",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                schema: "dbo",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaidAt",
                schema: "dbo",
                table: "Payments",
                column: "PaidAt");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Products_ProductCategoryId",
            //    schema: "dbo",
            //    table: "Products",
            //    column: "ProductCategoryId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Products_Title",
            //    schema: "dbo",
            //    table: "Products",
            //    column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_BranchId_CreatedAt",
                schema: "dbo",
                table: "StockTransactions",
                columns: new[] { "BranchId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_BranchId_ProductId_CreatedAt",
                schema: "dbo",
                table: "StockTransactions",
                columns: new[] { "BranchId", "ProductId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_OrderId",
                schema: "dbo",
                table: "StockTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_ProductId",
                schema: "dbo",
                table: "StockTransactions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActionLogs_BranchId_OccurredAt",
                schema: "dbo",
                table: "UserActionLogs",
                columns: new[] { "BranchId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserActionLogs_UserId_OccurredAt",
                schema: "dbo",
                table: "UserActionLogs",
                columns: new[] { "UserId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserBranches_BranchId",
                schema: "dbo",
                table: "UserBranches",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBranches_UserId_BranchId",
                schema: "dbo",
                table: "UserBranches",
                columns: new[] { "UserId", "BranchId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BranchInventories",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CashLedgerEntries",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "IdentityRole");

            migrationBuilder.DropTable(
                name: "InventoryTransfers",
                schema: "dbo");

            //migrationBuilder.DropTable(
            //    name: "Medias",
            //    schema: "dbo");

            migrationBuilder.DropTable(
                name: "OrderItems",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserActionLogs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserBranches",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "LedgerCategories",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "StockTransactions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "dbo");

            //migrationBuilder.DropTable(
            //    name: "Products",
            //    schema: "dbo");

            migrationBuilder.DropTable(
                name: "Branches",
                schema: "dbo");

            //migrationBuilder.DropTable(
            //    name: "ProductCategories",
            //    schema: "dbo");
        }
    }
}
