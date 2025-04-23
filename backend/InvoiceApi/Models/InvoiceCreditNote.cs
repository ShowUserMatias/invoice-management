namespace InvoiceApi.Models
{
    public class InvoiceCreditNote
    {
        public int InvoiceCreditNoteId { get; set; }
        public int CreditNoteNumber { get; set; }
        public DateTime CreditNoteDate { get; set; }
        public decimal CreditNoteAmount { get; set; }
    }
}
