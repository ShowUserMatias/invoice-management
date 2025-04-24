// Dtos/InvoiceDto.cs
namespace InvoiceApi.Dtos
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public int InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string InvoiceStatus { get; set; } = string.Empty;
        public CustomerDto Customer { get; set; } = new();
        public List<InvoiceDetailDto> InvoiceDetails { get; set; } = new();
        public List<InvoiceCreditNoteDto> InvoiceCreditNotes { get; set; } = new();
        public InvoicePaymentDto? InvoicePayment { get; set; }
    }

    public class CustomerDto
    {
        public string CustomerRun { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
    }

    public class InvoiceDetailDto
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class InvoiceCreditNoteDto
    {
        public int CreditNoteNumber { get; set; }
        public DateTime CreditNoteDate { get; set; }
        public decimal CreditNoteAmount { get; set; }
    }

    public class InvoicePaymentDto
    {
        public string? PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
