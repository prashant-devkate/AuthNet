using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthNet.Migrations
{
    /// <inheritdoc />
    public partial class SalesTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AfterTaxAmount",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedAmount",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrincipalAmount",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 18, 14, 35, 45, 973, DateTimeKind.Utc).AddTicks(4025));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AfterTaxAmount",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "DiscountedAmount",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PrincipalAmount",
                table: "Sales");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 18, 10, 40, 21, 107, DateTimeKind.Utc).AddTicks(2175));
        }
    }
}
