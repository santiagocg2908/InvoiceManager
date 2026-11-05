using InvoiceManager.Application.Abstractions.Persistence;
using InvoiceManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvoiceManager.Infrastructure.Persistence.Repositories
{
    public sealed class SupplierRepository : ISupplierRepository
    {
        private readonly InvoiceManagerDbContext _context;

        public SupplierRepository(InvoiceManagerDbContext context)
        {
            _context = context;
        }

        public void Add(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
        }

        public async Task<Supplier?> GetByNormalizedNameAsync(string normalizedName,
            CancellationToken cancellationToken = default)
        {
            return await _context.Suppliers.FirstOrDefaultAsync(
                supplier => supplier.NormalizedName == normalizedName,
                cancellationToken
                );
        }
    }
}
