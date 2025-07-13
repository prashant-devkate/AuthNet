using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthNet.Migrations
{
    /// <inheritdoc />
    public partial class AddExtraColumnsToCompanyInfoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "CompanyInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 13, 4, 10, 54, 709, DateTimeKind.Utc).AddTicks(2057));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "CompanyInfos");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 13, 1, 55, 0, 896, DateTimeKind.Utc).AddTicks(1838));
        }
    }
}
