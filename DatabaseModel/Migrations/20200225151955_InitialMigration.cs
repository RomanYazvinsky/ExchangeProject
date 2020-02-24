using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exchange.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CountryCode = table.Column<string>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    PostIndex = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Abbreviation = table.Column<string>(nullable: false),
                    CurrencySign = table.Column<string>(nullable: false),
                    CountryCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeasureUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ShortName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasureUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductClasses",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductClasses", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Sellers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SellerName = table.Column<string>(nullable: false),
                    AddressId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sellers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sellers_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeasureUnitConversions",
                columns: table => new
                {
                    FromId = table.Column<Guid>(nullable: false),
                    ToId = table.Column<Guid>(nullable: false),
                    Coefficient = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasureUnitConversions", x => new { x.FromId, x.ToId });
                    table.ForeignKey(
                        name: "FK_MeasureUnitConversions_MeasureUnits_FromId",
                        column: x => x.FromId,
                        principalTable: "MeasureUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeasureUnitConversions_MeasureUnits_ToId",
                        column: x => x.ToId,
                        principalTable: "MeasureUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductClassAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    AssociatedClassId = table.Column<string>(nullable: false),
                    ValueDataType = table.Column<int>(nullable: false, defaultValue: 0),
                    Mandatory = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductClassAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductClassAttributes_ProductClasses_AssociatedClassId",
                        column: x => x.AssociatedClassId,
                        principalTable: "ProductClasses",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    SellerId = table.Column<Guid>(nullable: true),
                    DiscountId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: false),
                    IsEmailConfirmed = table.Column<bool>(nullable: false),
                    SellerId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductClassAttributeValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    ProductClassAttributeId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductClassAttributeValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductClassAttributeValues_ProductClassAttributes_ProductCl~",
                        column: x => x.ProductClassAttributeId,
                        principalTable: "ProductClassAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductClassAttributeValues_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductProductClassEntity",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(nullable: false),
                    ProductClassId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductProductClassEntity", x => new { x.ProductId, x.ProductClassId });
                    table.ForeignKey(
                        name: "FK_ProductProductClassEntity_ProductClasses_ProductClassId",
                        column: x => x.ProductClassId,
                        principalTable: "ProductClasses",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductProductClassEntity_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillModifierEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillModifierEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillModifierEntity_Sellers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Sellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillModifierEntity_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillModifierEntity_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Quantity = table.Column<decimal>(nullable: false),
                    Bill = table.Column<decimal>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    AddressId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValueChangeRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProductClassAttributeValueId = table.Column<Guid>(nullable: false),
                    SenderId = table.Column<Guid>(nullable: true),
                    ResolutionTime = table.Column<DateTime>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    PreviousValue = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValueChangeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValueChangeRequests_ProductClassAttributeValues_ProductClass~",
                        column: x => x.ProductClassAttributeValueId,
                        principalTable: "ProductClassAttributeValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ValueChangeRequests_Sellers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Sellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderToBillModifier",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(nullable: false),
                    BillModifierId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderToBillModifier", x => new { x.OrderId, x.BillModifierId });
                    table.ForeignKey(
                        name: "FK_OrderToBillModifier_BillModifierEntity_BillModifierId",
                        column: x => x.BillModifierId,
                        principalTable: "BillModifierEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderToBillModifier_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OrderId = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    CurrencyId = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TransactionStart = table.Column<DateTime>(nullable: false),
                    TransactionEnd = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTransactions_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderTransactions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Abbreviation", "CountryCode", "CurrencySign", "Name" },
                values: new object[] { new Guid("3726f16b-1aef-443a-9762-24bdd62cd530"), "USD", "840", "$", "United States Dollar" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "IsEmailConfirmed", "PasswordHash", "Role", "SellerId", "Username" },
                values: new object[] { new Guid("0041bdea-26ea-42b1-9702-2737636886a8"), "admin@site-exchange.com", true, "a6Cc8rPnIK79vPOT1/1O5AW3kpA7iuLfVevKZwX7YJv6nqFTMnAxB9/DXwm0hkh0do//ipyqTOt8QJEsJn10Ebq1dE+Fj+k0GW4cloYC+/bhRAJw3EcDnTKCxCk3DRSbLXRE4l71HWJu9eZHBCSV/3wwGVaheSndTdJfjxO6gJ0=.74by0ZfpL7E5bENbz6J46g==", "Administrator", null, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_BillModifierEntity_OwnerId",
                table: "BillModifierEntity",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BillModifierEntity_ProductId",
                table: "BillModifierEntity",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillModifierEntity_OwnerId1",
                table: "BillModifierEntity",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasureUnitConversions_ToId",
                table: "MeasureUnitConversions",
                column: "ToId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AddressId",
                table: "Orders",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId",
                table: "Orders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderToBillModifier_BillModifierId",
                table: "OrderToBillModifier",
                column: "BillModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTransactions_CurrencyId",
                table: "OrderTransactions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTransactions_OrderId",
                table: "OrderTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductClassAttributes_AssociatedClassId",
                table: "ProductClassAttributes",
                column: "AssociatedClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductClassAttributes_Name_AssociatedClassId",
                table: "ProductClassAttributes",
                columns: new[] { "Name", "AssociatedClassId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductClassAttributeValues_ProductClassAttributeId",
                table: "ProductClassAttributeValues",
                column: "ProductClassAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductClassAttributeValues_ProductId_ProductClassAttributeId",
                table: "ProductClassAttributeValues",
                columns: new[] { "ProductId", "ProductClassAttributeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductProductClassEntity_ProductClassId",
                table: "ProductProductClassEntity",
                column: "ProductClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SellerId",
                table: "Products",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_AddressId",
                table: "Sellers",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SellerId",
                table: "Users",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ValueChangeRequests_ProductClassAttributeValueId",
                table: "ValueChangeRequests",
                column: "ProductClassAttributeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_ValueChangeRequests_SenderId",
                table: "ValueChangeRequests",
                column: "SenderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasureUnitConversions");

            migrationBuilder.DropTable(
                name: "OrderToBillModifier");

            migrationBuilder.DropTable(
                name: "OrderTransactions");

            migrationBuilder.DropTable(
                name: "ProductProductClassEntity");

            migrationBuilder.DropTable(
                name: "ValueChangeRequests");

            migrationBuilder.DropTable(
                name: "MeasureUnits");

            migrationBuilder.DropTable(
                name: "BillModifierEntity");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProductClassAttributeValues");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ProductClassAttributes");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProductClasses");

            migrationBuilder.DropTable(
                name: "Sellers");

            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}
