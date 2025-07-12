using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthNet.Migrations
{
    /// <inheritdoc />
    public partial class FlattenedPurchaseOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 12, 4, 26, 27, 355, DateTimeKind.Utc).AddTicks(3502));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 11, 14, 33, 4, 746, DateTimeKind.Utc).AddTicks(997));
        }
    }
}
