namespace InvoiceApi.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;

        // Relaciones
        public Customer Customer { get; set; } = null!;
        public List<InvoiceDetail> InvoiceDetails { get; set; } = new();
        public List<InvoiceCreditNote> InvoiceCreditNotes { get; set; } = new();
        public InvoicePayment InvoicePayment { get; set; } = null!;
    }
}
