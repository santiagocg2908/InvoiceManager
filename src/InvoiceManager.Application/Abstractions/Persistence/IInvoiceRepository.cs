using InvoiceManager.Application.DTOs.Common;
using InvoiceManager.Application.DTOs.Invoices;
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
        Task<PagedResult<Invoice>> GetPagedAsync(
            InvoiceListQuery query,
            CancellationToken cancellationToken = default);
        Task<Invoice?> GetByIdForUpdateAsync(
            int id, 
            CancellationToken cancellationToken=default
            );
        Task<bool> ExistsForAnotherInvoiceAsync(
            int supplierId,
            string normalizedInvoiceNumber,
            int excludedInvoiceId,
            CancellationToken cancellationToken = default
        );
    }
}
