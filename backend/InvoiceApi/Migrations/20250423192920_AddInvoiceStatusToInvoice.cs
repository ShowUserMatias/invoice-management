using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceApi.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceStatusToInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceStatus",
                table: "Invoices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceStatus",
                table: "Invoices");
        }
    }
}
