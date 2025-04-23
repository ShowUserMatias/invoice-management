using InvoiceApi.Data;
using InvoiceApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            Console.WriteLine($"JSON leído correctamente. Primeros 300 caracteres:\n{jsonContent.Substring(0, Math.Min(300, jsonContent.Length))}");

            var jsonData = JsonSerializer.Deserialize<JsonStructure>(
                jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (jsonData?.Invoices == null || jsonData.Invoices.Count == 0)
            {
                return "El archivo JSON no tiene datos válidos.";
            }

            Console.WriteLine($"Se encontraron {jsonData.Invoices.Count} facturas en el JSON.");

            int insertedCount = 0;
            int skippedCount = 0;
            List<string> skipReasons = new();

            foreach (var invoiceJson in jsonData.Invoices)
            {
                bool exists = await _context.Invoices.AnyAsync(i => i.InvoiceNumber == invoiceJson.InvoiceNumber);
                if (exists)
                {
                    string reason = $"Factura {invoiceJson.InvoiceNumber} omitida: ya existe en la base de datos.";
                    Console.WriteLine(reason);
                    skipReasons.Add(reason);
                    skippedCount++;
                    continue;
                }

                decimal subtotalSum = invoiceJson.InvoiceDetail.Sum(d => d.Subtotal);
                if (subtotalSum != invoiceJson.TotalAmount)
                {
                    string reason = $"Factura {invoiceJson.InvoiceNumber} omitida: suma de subtotales ({subtotalSum}) no coincide con el total ({invoiceJson.TotalAmount}).";
                    Console.WriteLine(reason);
                    skipReasons.Add(reason);
                    skippedCount++;
                    continue;
                }

                Console.WriteLine($"Factura {invoiceJson.InvoiceNumber} será insertada.");

                var customer = new Customer
                {
                    CustomerRun = invoiceJson.Customer.CustomerRun,
                    CustomerName = invoiceJson.Customer.CustomerName,
                    CustomerEmail = invoiceJson.Customer.CustomerEmail
                };

                var details = invoiceJson.InvoiceDetail.Select(d => new InvoiceDetail
                {
                    ProductName = d.ProductName,
                    UnitPrice = d.UnitPrice,
                    Quantity = d.Quantity,
                    Subtotal = d.Subtotal
                }).ToList();

                var creditNotes = invoiceJson.InvoiceCreditNote.Select(nc => new InvoiceCreditNote
                {
                    CreditNoteNumber = nc.CreditNoteNumber,
                    CreditNoteDate = nc.CreditNoteDate,
                    CreditNoteAmount = nc.CreditNoteAmount
                }).ToList();

                var payment = new InvoicePayment
                {
                    PaymentMethod = invoiceJson.InvoicePayment.PaymentMethod,
                    PaymentDate = invoiceJson.InvoicePayment.PaymentDate
                };

                decimal totalNc = creditNotes.Sum(nc => nc.CreditNoteAmount);
                string invoiceStatus = totalNc == invoiceJson.TotalAmount ? "Cancelled" : totalNc > 0 ? "Partial" : "Issued";
                string paymentStatus = payment.PaymentDate.HasValue ? "Paid" : DateTime.Now.Date > invoiceJson.PaymentDueDate ? "Overdue" : "Pending";

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
            Console.WriteLine($"Se omitieron {skippedCount} facturas por duplicadas o inconsistentes.");

            string resultMessage = $"Proceso de carga finalizado. Insertadas: {insertedCount}, Omitidas: {skippedCount}.\n";
            if (skipReasons.Count > 0)
            {
                resultMessage += "Motivos de omisión:\n" + string.Join("\n", skipReasons);
            }

            return resultMessage;
        }
    }

    // Estructuras auxiliares para deserializar el JSON:
    public class JsonStructure
    {
        public List<InvoiceJson> Invoices { get; set; } = new();
    }

    public class InvoiceJson
    {
        [JsonPropertyName("invoice_number")]
        public int InvoiceNumber { get; set; }

        [JsonPropertyName("invoice_date")]
        public DateTime InvoiceDate { get; set; }

        [JsonPropertyName("total_amount")]
        public decimal TotalAmount { get; set; }

        [JsonPropertyName("payment_due_date")]
        public DateTime PaymentDueDate { get; set; }

        [JsonPropertyName("payment_status")]
        public string PaymentStatus { get; set; } = string.Empty;

        [JsonPropertyName("invoice_status")]
        public string InvoiceStatus { get; set; } = string.Empty;

        [JsonPropertyName("invoice_detail")]
        public List<InvoiceDetailJson> InvoiceDetail { get; set; } = new();

        [JsonPropertyName("invoice_payment")]
        public InvoicePaymentJson InvoicePayment { get; set; } = new();

        [JsonPropertyName("invoice_credit_note")]
        public List<InvoiceCreditNoteJson> InvoiceCreditNote { get; set; } = new();

        [JsonPropertyName("customer")]
        public CustomerJson Customer { get; set; } = new();
    }

    public class InvoiceDetailJson
    {
        [JsonPropertyName("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [JsonPropertyName("unit_price")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("subtotal")]
        public decimal Subtotal { get; set; }
    }

    public class InvoiceCreditNoteJson
    {
        [JsonPropertyName("credit_note_number")]
        public int CreditNoteNumber { get; set; }

        [JsonPropertyName("credit_note_date")]
        public DateTime CreditNoteDate { get; set; }

        [JsonPropertyName("credit_note_amount")]
        public decimal CreditNoteAmount { get; set; }
    }

    public class InvoicePaymentJson
    {
        [JsonPropertyName("payment_method")]
        public string? PaymentMethod { get; set; }

        [JsonPropertyName("payment_date")]
        public DateTime? PaymentDate { get; set; }
    }

    public class CustomerJson
    {
        [JsonPropertyName("customer_run")]
        public string CustomerRun { get; set; } = string.Empty;

        [JsonPropertyName("customer_name")]
        public string CustomerName { get; set; } = string.Empty;

        [JsonPropertyName("customer_email")]
        public string CustomerEmail { get; set; } = string.Empty;
    }
}
