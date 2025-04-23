using InvoiceApi.Data;
using InvoiceApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InvoiceApi.Services
{
    public class JsonLoaderService
    {
        private readonly AppDbContext _context;

        public JsonLoaderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> LoadInvoicesFromJsonAsync(string jsonFilePath)
        {
            if (!File.Exists(jsonFilePath))
            {
                return "Archivo JSON no encontrado.";
            }

            var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
            var jsonData = JsonSerializer.Deserialize<JsonStructure>(jsonContent);

            if (jsonData?.Invoices == null)
            {
                return "El archivo JSON no tiene datos válidos.";
            }

            foreach (var invoiceJson in jsonData.Invoices)
            {
                // Validar invoice_number único
                bool exists = await _context.Invoices.AnyAsync(i => i.InvoiceNumber == invoiceJson.InvoiceNumber);
                if (exists)
                {
                    continue; // Saltar si ya existe
                }

                // Validar suma de subtotales
                decimal subtotalSum = invoiceJson.InvoiceDetail.Sum(d => d.Subtotal);
                if (subtotalSum != invoiceJson.TotalAmount)
                {
                    continue; // Saltar si hay incoherencia
                }

                // Mapear Customer
                var customer = new Customer
                {
                    CustomerRun = invoiceJson.Customer.CustomerRun,
                    CustomerName = invoiceJson.Customer.CustomerName,
                    CustomerEmail = invoiceJson.Customer.CustomerEmail
                };

                // Mapear detalles
                var details = invoiceJson.InvoiceDetail.Select(d => new InvoiceDetail
                {
                    ProductName = d.ProductName,
                    UnitPrice = d.UnitPrice,
                    Quantity = d.Quantity,
                    Subtotal = d.Subtotal
                }).ToList();

                // Mapear notas de crédito
                var creditNotes = invoiceJson.InvoiceCreditNote.Select(nc => new InvoiceCreditNote
                {
                    CreditNoteNumber = nc.CreditNoteNumber,
                    CreditNoteDate = nc.CreditNoteDate,
                    CreditNoteAmount = nc.CreditNoteAmount
                }).ToList();

                // Mapear pago
                var payment = new InvoicePayment
                {
                    PaymentMethod = invoiceJson.InvoicePayment.PaymentMethod,
                    PaymentDate = invoiceJson.InvoicePayment.PaymentDate
                };

                // Calcular estado de factura
                decimal totalNc = creditNotes.Sum(nc => nc.CreditNoteAmount);
                string invoiceStatus = "Issued";
                if (totalNc == invoiceJson.TotalAmount)
                {
                    invoiceStatus = "Cancelled";
                }
                else if (totalNc > 0 && totalNc < invoiceJson.TotalAmount)
                {
                    invoiceStatus = "Partial";
                }

                // Calcular estado de pago
                string paymentStatus = "Pending";
                if (payment.PaymentDate.HasValue)
                {
                    paymentStatus = "Paid";
                }
                else if (DateTime.Now.Date > invoiceJson.PaymentDueDate)
                {
                    paymentStatus = "Overdue";
                }

                // Crear entidad Invoice
                var invoice = new Invoice
                {
                    InvoiceNumber = invoiceJson.InvoiceNumber,
                    InvoiceDate = invoiceJson.InvoiceDate,
                    TotalAmount = invoiceJson.TotalAmount,
                    PaymentDueDate = invoiceJson.PaymentDueDate,
                    PaymentStatus = paymentStatus,
                    Customer = customer,
                    InvoiceDetails = details,
                    InvoiceCreditNotes = creditNotes,
                    InvoicePayment = payment
                };

                await _context.Invoices.AddAsync(invoice);
            }

            await _context.SaveChangesAsync();
            return "Proceso de carga finalizado.";
        }
    }

    // Estructuras auxiliares para deserializar el JSON:
    public class JsonStructure
    {
        public List<InvoiceJson> Invoices { get; set; } = new();
    }

    public class InvoiceJson
    {
        public int InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public List<InvoiceDetailJson> InvoiceDetail { get; set; } = new();
        public InvoicePaymentJson InvoicePayment { get; set; } = new();
        public List<InvoiceCreditNoteJson> InvoiceCreditNote { get; set; } = new();
        public CustomerJson Customer { get; set; } = new();
    }

    public class InvoiceDetailJson
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class InvoiceCreditNoteJson
    {
        public int CreditNoteNumber { get; set; }
        public DateTime CreditNoteDate { get; set; }
        public decimal CreditNoteAmount { get; set; }
    }

    public class InvoicePaymentJson
    {
        public string? PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
    }

    public class CustomerJson
    {
        public string CustomerRun { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
    }
}
