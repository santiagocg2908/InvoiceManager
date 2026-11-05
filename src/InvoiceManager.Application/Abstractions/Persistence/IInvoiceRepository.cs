using InvoiceManager.Domain.Entities;

namespace InvoiceManager.Application.Abstractions.Persistence
{
    public interface IInvoiceRepository
    {
        Task<bool> ExistAsync(
            int supplierId,
            string normalizedInvoiceNumber,
            CancellationToken cancellationToken = default);
        Task<Invoice?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken= default);
        void Add(Invoice invoice);
    }
}
