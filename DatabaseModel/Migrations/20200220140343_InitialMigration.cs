using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseModel.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Guid",
                keyValue: new Guid("21ae3cd1-79bc-4f6c-b6b6-ec916298c269"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Guid",
                keyValue: new Guid("6983a033-e12d-4c8d-a2df-f03df3ba4164"));

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailConfirmed",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Guid", "Abbreviation", "CountryCode", "CurrencySign", "Name" },
                values: new object[] { new Guid("26d25e9d-8c36-4fe0-af22-1e958d48d96d"), "USD", "840", "$", "United States Dollar" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Guid", "Email", "IsEmailConfirmed", "PasswordHash", "Role", "SellerId", "UserName" },
                values: new object[] { new Guid("6482c6d0-369e-46e4-983d-f51985b62638"), null, false, "5HRF/xi/gE0659tLUgnKTGQMu6WrksNH48kIvAZicWpVP4TC3HaG6WMEeZ0G0/XFdqnffcoISvpIrzF9tAxKxcl3mPtiQOvHAyvF2I6QWANOxUMMxYNVJ6tiTyJVyiJXD+bL4Ie/ewPZnaZxIkVcaB+o42Nk9WSfWKJnHVsNNMw=.Lg9rFoaQMdwCEF78YIkI8w==", "Administrator", null, "admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Guid",
                keyValue: new Guid("26d25e9d-8c36-4fe0-af22-1e958d48d96d"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Guid",
                keyValue: new Guid("6482c6d0-369e-46e4-983d-f51985b62638"));

            migrationBuilder.DropColumn(
                name: "IsEmailConfirmed",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Guid", "Abbreviation", "CountryCode", "CurrencySign", "Name" },
                values: new object[] { new Guid("21ae3cd1-79bc-4f6c-b6b6-ec916298c269"), "USD", "840", "$", "United States Dollar" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Guid", "Email", "PasswordHash", "Role", "SellerId", "UserName" },
                values: new object[] { new Guid("6983a033-e12d-4c8d-a2df-f03df3ba4164"), null, "5qBbtpOnndwMpBQMy7wHKNn+X7apP6uL2tbWCMF5W9kB1kXbfX4PQ/e6M7NZg9ikX5D87BaRNKAw21GeiWdm7E5A0cY2ZHm/+uVDhX69v/jPR8ZxsYWff6uI33BNEjEczgMHFGVPNeexChDwSOkdU2DxwnLvOvkdpyQ41NxU0/k=.r4jOHK32cleYtwBxveLsnQ==", "Administrator", null, "admin" });
        }
    }
}
