namespace InvoiceApi.Models
{
    public class InvoicePayment
    {
        public int InvoicePaymentId { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
