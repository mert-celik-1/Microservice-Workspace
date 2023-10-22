using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _2PC.Coordinator.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Nodes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("3ad156e6-aec2-44c9-98d3-6c951bc47717"), "Stock.API" },
                    { new Guid("72816ef7-383c-456c-aefa-b47befec9995"), "Payment.API" },
                    { new Guid("a72dfd95-6c11-4b5e-8b2a-5781f16990b1"), "Order.API" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("3ad156e6-aec2-44c9-98d3-6c951bc47717"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("72816ef7-383c-456c-aefa-b47befec9995"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("a72dfd95-6c11-4b5e-8b2a-5781f16990b1"));
        }
    }
}
