namespace InvoiceApi.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerRun { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
    }
}
