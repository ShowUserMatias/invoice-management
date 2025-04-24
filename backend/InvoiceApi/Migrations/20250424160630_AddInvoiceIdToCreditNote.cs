using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceApi.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceIdToCreditNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceCreditNotes_Invoices_InvoiceId",
                table: "InvoiceCreditNotes");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceId",
                table: "InvoiceCreditNotes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceCreditNotes_Invoices_InvoiceId",
                table: "InvoiceCreditNotes",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceCreditNotes_Invoices_InvoiceId",
                table: "InvoiceCreditNotes");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceId",
                table: "InvoiceCreditNotes",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceCreditNotes_Invoices_InvoiceId",
                table: "InvoiceCreditNotes",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "InvoiceId");
        }
    }
}
