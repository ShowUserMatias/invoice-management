using InvoiceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<InvoiceDetail> InvoiceDetails => Set<InvoiceDetail>();
        public DbSet<InvoiceCreditNote> InvoiceCreditNotes => Set<InvoiceCreditNote>();
        public DbSet<InvoicePayment> InvoicePayments => Set<InvoicePayment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Customer);

            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.InvoiceDetails);

            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.InvoiceCreditNotes);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.InvoicePayment);
        }
    }
}
