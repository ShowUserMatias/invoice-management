using InvoiceApi.Data;
using InvoiceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApi.Services
{
    public class InvoiceService
    {
        private readonly AppDbContext _context;

        public InvoiceService(AppDbContext context)
        {
            _context = context;
        }
                   
        private int GenerateCreditNoteNumber()
        {
            // Aquí puedes implementar una lógica mejor si es necesario
            return new Random().Next(100000, 999999);
        }
        public async Task<string> AddCreditNoteAsync(int invoiceId, int creditNoteNumber, decimal creditNoteAmount)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceCreditNotes)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            if (invoice == null)
            {
                return "Factura no encontrada.";
            }

            decimal totalCreditNotes = invoice.InvoiceCreditNotes.Sum(nc => nc.CreditNoteAmount);
            decimal remainingAmount = invoice.TotalAmount - totalCreditNotes;

            if (creditNoteAmount <= 0)
            {
                return "El monto de la nota de crédito debe ser mayor a cero.";
            }

            if (creditNoteAmount > remainingAmount)
            {
                return $"El monto de la nota de crédito ({creditNoteAmount}) supera el saldo pendiente de la factura ({remainingAmount}).";
            }

            var creditNote = new InvoiceCreditNote
            {
                InvoiceId = invoiceId,
                CreditNoteNumber = creditNoteNumber,
                CreditNoteDate = DateTime.Now,
                CreditNoteAmount = creditNoteAmount
            };

            _context.InvoiceCreditNotes.Add(creditNote);
            await _context.SaveChangesAsync();

            await UpdateInvoiceStatusAsync(invoiceId);

            return "Nota de crédito agregada exitosamente.";
        }

        private async Task UpdateInvoiceStatusAsync(int invoiceId)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceCreditNotes)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            if (invoice == null)
                return;

            decimal totalNc = invoice.InvoiceCreditNotes.Sum(nc => nc.CreditNoteAmount);

            if (totalNc == 0)
            {
                invoice.InvoiceStatus = "Issued";
            }
            else if (totalNc >= invoice.TotalAmount)
            {
                invoice.InvoiceStatus = "Cancelled";
            }
            else
            {
                invoice.InvoiceStatus = "Partial";
            }

            await _context.SaveChangesAsync();
        }
    }
}
