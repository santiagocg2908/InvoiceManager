using InvoiceManager.Application.Abstractions.Persistence;
using InvoiceManager.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Infrastructure.Persistence
{
    public class InvoiceManagerDbContext: DbContext, IUnitOfWork
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

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) 
                when (IsUniqueConstraintViolation(
                    exception, "UX_Invoices_Supplier_InvoiceNumber"))
            {
                throw new DuplicateWaitObjectException(
                    "There is alredy one invoice with that supplier and number",
                    exception
                    );
            }
            catch (DbUpdateException exception)
                when(IsUniqueConstraintViolation(
                    exception, "UX_Suppliers_NormalizedName"))
            {
                throw new DuplicateWaitObjectException(
                    "There is alredy one supplier with that normalized name"
                    );
            }
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException exception,
            string indexName)
        {
            var sqlException = exception.GetBaseException() as SqlException;

            return sqlException is not null &&
                (sqlException.Number == 2601 ||
                sqlException.Number == 2627) &&
                sqlException.Message.Contains(
                    indexName,
                    StringComparison.OrdinalIgnoreCase
                    );
        }
    }
}
