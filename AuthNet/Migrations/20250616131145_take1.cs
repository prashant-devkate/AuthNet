using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthNet.Migrations
{
    /// <inheritdoc />
    public partial class take1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_purchaseOrders_Users_CreatedByUserId",
                table: "purchaseOrders");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 16, 13, 11, 44, 207, DateTimeKind.Utc).AddTicks(8927));

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchaseOrders_Users_CreatedByUserId",
                table: "purchaseOrders",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_purchaseOrders_Users_CreatedByUserId",
                table: "purchaseOrders");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 16, 9, 25, 28, 24, DateTimeKind.Utc).AddTicks(5189));

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_purchaseOrders_Users_CreatedByUserId",
                table: "purchaseOrders",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
