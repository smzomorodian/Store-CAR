using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrustructure.Migrations
{
    /// <inheritdoc />
    public partial class drop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Sales");

            migrationBuilder.AddColumn<Guid>(
                name: "BuyerId",
                table: "Sales",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Sales_BuyerId",
                table: "Sales",
                column: "BuyerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_buyers_BuyerId",
                table: "Sales",
                column: "BuyerId",
                principalTable: "buyers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_buyers_BuyerId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_BuyerId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "Sales");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
