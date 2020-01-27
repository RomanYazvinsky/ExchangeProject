using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseModel.Migrations
{
    public partial class IntialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasureUnits",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ShortName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasureUnits", x => x.Guid);
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
                name: "Users",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Guid);
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
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeasureUnitConversions_MeasureUnits_ToId",
                        column: x => x.ToId,
                        principalTable: "MeasureUnits",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductClassAttributes",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    AssociatedClassId = table.Column<string>(nullable: true),
                    ValueDataType = table.Column<int>(nullable: false, defaultValue: 0),
                    Mandatory = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductClassAttributes", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ProductClassAttributes_ProductClasses_AssociatedClassId",
                        column: x => x.AssociatedClassId,
                        principalTable: "ProductClasses",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Products_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserDeviceLogins",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    ValidUntil = table.Column<DateTime>(nullable: false),
                    DeviceInfo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDeviceLogins", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_UserDeviceLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductClassAttributeValues",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    ProductClassAttributeId = table.Column<Guid>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductClassAttributeValues", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ProductClassAttributeValues_ProductClassAttributes_ProductCl~",
                        column: x => x.ProductClassAttributeId,
                        principalTable: "ProductClassAttributes",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductClassAttributeValues_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductProductClass",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(nullable: false),
                    ProductClassId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductProductClass", x => new { x.ProductId, x.ProductClassId });
                    table.ForeignKey(
                        name: "FK_ProductProductClass_ProductClasses_ProductClassId",
                        column: x => x.ProductClassId,
                        principalTable: "ProductClasses",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductProductClass_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValueChangeRequests",
                columns: table => new
                {
                    Guid = table.Column<Guid>(nullable: false),
                    ProductClassAttributeValueId = table.Column<Guid>(nullable: false),
                    SenderId = table.Column<Guid>(nullable: true),
                    ResolutionTime = table.Column<DateTime>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    PreviousValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValueChangeRequests", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_ValueChangeRequests_ProductClassAttributeValues_ProductClass~",
                        column: x => x.ProductClassAttributeValueId,
                        principalTable: "ProductClassAttributeValues",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ValueChangeRequests_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Guid", "Email", "PasswordHash", "Role", "UserName" },
                values: new object[] { new Guid("5abded61-4445-44f6-835b-f40f64ac417f"), null, "22ZYYZAcuxsrN3ODp6OcswRV1Tp0YgV7dFqnh/eMG6uLC6ZI+IlsrzxBdlxTaZydZxOuNmOfgMQUcCikMev6aPxpDsV7uK9OIb/Jes8vcj9Ot1Ep0WID9/GApy/4Fbq/vI3pKRMiFw1YdEJ7/AwcHqwV0Lcg5Ga1THtvcCbpmxk=.7GYs99cuf/6jo5AQ50/HGg==", "Administrator", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_MeasureUnitConversions_ToId",
                table: "MeasureUnitConversions",
                column: "ToId");

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
                name: "IX_ProductProductClass_ProductClassId",
                table: "ProductProductClass",
                column: "ProductClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UserId",
                table: "Products",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDeviceLogins_UserId",
                table: "UserDeviceLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
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
                name: "ProductProductClass");

            migrationBuilder.DropTable(
                name: "UserDeviceLogins");

            migrationBuilder.DropTable(
                name: "ValueChangeRequests");

            migrationBuilder.DropTable(
                name: "MeasureUnits");

            migrationBuilder.DropTable(
                name: "ProductClassAttributeValues");

            migrationBuilder.DropTable(
                name: "ProductClassAttributes");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProductClasses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
