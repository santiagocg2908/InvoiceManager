using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InvoiceManager.Application.Abstractions.Persistence;
using InvoiceManager.Application.DTOs.Common;
using InvoiceManager.Application.DTOs.Invoices;
using InvoiceManager.Domain.Entities;


namespace InvoiceManager.Application.UseCases.Invoices
{
    public sealed class ListInvoicesUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        
        public ListInvoicesUseCase(
            IInvoiceRepository invoiceRepository
            ) 
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<PagedResult<InvoiceListItemDto>> ExecuteAsync(
            InvoiceListQuery query,
            CancellationToken cancellationToken= default
            )
        {
            ArgumentNullException.ThrowIfNull(query);

            if (query.Page < 1)
            {
                throw new ArgumentOutOfRangeException(
                    "The page must be greater than zero",
                    nameof(query.Page)
                    );
            }

            if (query.PageSize < 1 || query.PageSize > 100)
            {
                throw new ArgumentOutOfRangeException(
                    "The Page size must be betwee 1 and 100",
                    nameof(query.PageSize)
                    );
            }

            if (query.Supplier?.Trim().Length>200)
            {
                throw new ArgumentException(
                    "The supplier filter cannot exceed 200 characters",
                    nameof(query.Supplier)
                    );
            }

            if (query.IssueDateFrom.HasValue &&
                query.IssueDateTo.HasValue &&
                query.IssueDateFrom.Value > query.IssueDateTo.Value
                )
            {
                throw new ArgumentException(
                    "The start date cannot be later than the end date",
                    nameof(query)
                    );
            }

            PagedResult<Invoice> pagedInvoices =
                await _invoiceRepository.GetPagedAsync(
                    query,
                    cancellationToken
                    );

            var invoiceItems = pagedInvoices.Items
                .Select(invoice => new InvoiceListItemDto(
                    invoice.Id,
                    invoice.Supplier.Name,
                    invoice.InvoiceNumber,
                    invoice.IssueDate,
                    invoice.DueDate,
                    invoice.TotalAmount,
                    invoice.Currency,
                    invoice.IsActive
                ))
                .ToList();
            return new PagedResult<InvoiceListItemDto>(
                invoiceItems,
                pagedInvoices.Page,
                pagedInvoices.PageSize,
                pagedInvoices.TotalItems
                );
        }
    }
}
