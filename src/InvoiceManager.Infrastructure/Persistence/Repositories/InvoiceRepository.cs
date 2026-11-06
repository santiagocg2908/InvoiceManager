using InvoiceManager.Application.Abstractions.Persistence;
using InvoiceManager.Application.DTOs.Common;
using InvoiceManager.Application.DTOs.Invoices;
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

        public async Task<PagedResult<Invoice>> GetPagedAsync(InvoiceListQuery query,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(query);

            IQueryable<Invoice> invoices = _context
                .Set<Invoice>()
                .AsNoTracking()
                .Include(invoice => invoice.Supplier);

            if (!query.IncludeInactive) 
            {
                invoices = invoices.Where(invoice => invoice.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(query.Supplier))
            {
                string supplier = query.Supplier.Trim();

                invoices = invoices.Where(invoice =>
                    invoice.Supplier.Name.Contains(supplier));
            }

            if (query.IssueDateFrom.HasValue)
            {
                invoices = invoices.Where(invoice =>
                    invoice.IssueDate >= query.IssueDateFrom.Value
                );
            }

            if (query.IssueDateTo.HasValue)
            {
                invoices = invoices.Where(invoice =>
                    invoice.IssueDate <= query.IssueDateTo.Value
                    );
            }

            int totalItems = await invoices.CountAsync(
                cancellationToken
                );

            int skip = checked(
                (query.Page - 1) * query.PageSize
                );

            List<Invoice> items = await invoices
                .OrderByDescending(invoice => invoice.IssueDate)
                .ThenByDescending(invoice => invoice.Id)
                .Skip(skip)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Invoice>(
                items,
                query.Page,
                query.PageSize,
                totalItems
                );
        }
    }
}
