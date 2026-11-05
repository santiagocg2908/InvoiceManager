using InvoiceManager.Application.Abstractions.Persistence;
using InvoiceManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace InvoiceManager.Infrastructure.Persistence.Repositories
{
    public sealed class InvoiceRepository : IInvoiceRepository
    {
        private readonly InvoiceManagerDbContext _context;

        public InvoiceRepository(InvoiceManagerDbContext context)
        {
            _context = context;
        }


        public void Add(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
        }

        public async Task<bool> ExistAsync(int supplierId,
            string normalizedInvoiceNumber,
            CancellationToken cancellationToken = default)
        {
            return await _context.Invoices.AnyAsync(
                invoice=>
                    invoice.SupplierId == supplierId &&
                    invoice.NormalizedInvoiceNumber == normalizedInvoiceNumber,
                cancellationToken
                );
        }

        public async Task<Invoice?> GetByIdAsync(int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Invoices
                .Include(invoice => invoice.Supplier)
                .Include(invoice => invoice.Document)
                .FirstOrDefaultAsync(
                    invoice => invoice.Id == id,
                    cancellationToken
                );
        }
    }
}
