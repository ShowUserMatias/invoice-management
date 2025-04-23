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

            Console.WriteLine($"✅ JSON leído correctamente. Primeros 300 caracteres:\n{jsonContent.Substring(0, Math.Min(300, jsonContent.Length))}");

            var jsonData = JsonSerializer.Deserialize<JsonStructure>(
                jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (jsonData?.Invoices == null || jsonData.Invoices.Count == 0)
            {
                Console.WriteLine("El archivo se leyó, pero no se encontraron facturas dentro de 'Invoices'.");
                return "El archivo JSON no tiene datos válidos.";
            }
            else
            {
                Console.WriteLine($"Se encontraron {jsonData.Invoices.Count} facturas en el JSON.");
            }            

            if (jsonData?.Invoices == null)
            {
                return "El archivo JSON no tiene datos válidos.";
            }

            int insertedCount = 0;
            int skippedCount = 0;

            foreach (var invoiceJson in jsonData.Invoices)
            {
                bool exists = await _context.Invoices.AnyAsync(i => i.InvoiceNumber == invoiceJson.InvoiceNumber);
                if (exists)
                {
                    Console.WriteLine($"Factura {invoiceJson.InvoiceNumber} ya existe. Saltando...");
                    skippedCount++;
                    continue;
                }

                decimal subtotalSum = invoiceJson.InvoiceDetail.Sum(d => d.Subtotal);
                if (subtotalSum != invoiceJson.TotalAmount)
                {
                    Console.WriteLine($"Factura {invoiceJson.InvoiceNumber} inconsistente: suma de subtotales {subtotalSum} != total {invoiceJson.TotalAmount}");
                    skippedCount++;
                    continue;
                }

                Console.WriteLine($"Factura {invoiceJson.InvoiceNumber} será insertada.");

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
                    InvoiceStatus = invoiceStatus,
                    Customer = customer,
                    InvoiceDetails = details,
                    InvoiceCreditNotes = creditNotes,
                    InvoicePayment = payment
                };

                await _context.Invoices.AddAsync(invoice);
                insertedCount++;
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"Se insertaron {insertedCount} facturas correctamente.");
            Console.WriteLine($"e omitieron {skippedCount} facturas por duplicadas o inconsistentes.");

            return $"Proceso de carga finalizado. Insertadas: {insertedCount}, Omitidas: {skippedCount}.";
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
