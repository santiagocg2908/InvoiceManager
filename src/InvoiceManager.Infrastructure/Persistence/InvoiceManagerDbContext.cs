using InvoiceManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Infrastructure.Persistence
{
    public class InvoiceManagerDbContext: DbContext
    {
        //Constructor 
        public InvoiceManagerDbContext(
            DbContextOptions<InvoiceManagerDbContext>options): 
            base(options)
        {
        }
        //Entities
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceDocument> InvoiceDocuments => Set<InvoiceDocument>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(InvoiceManagerDbContext).Assembly);
        }

    }
}
