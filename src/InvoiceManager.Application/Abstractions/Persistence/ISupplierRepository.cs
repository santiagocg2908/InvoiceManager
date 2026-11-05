using InvoiceManager.Domain.Entities;

namespace InvoiceManager.Application.Abstractions.Persistence
{
    public interface ISupplierRepository
    {
        Task<Supplier?> GetByNormalizedNameAsync(
            string normalizedName,
            CancellationToken cancellationToken = default
            );
        void Add(Supplier supplier);
    }
}
