using System;
using System.Threading;
using System.Threading.Tasks;
using InvoiceManager.Application.Abstractions.Persistence;
using InvoiceManager.Application.DTOs.Invoices;
using InvoiceManager.Domain.Entities;


namespace InvoiceManager.Application.UseCases.Invoices
{
    public sealed class GetInvoiceDetailsUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetInvoiceDetailsUseCase(
            IInvoiceRepository invoiceRepository
            )
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<InvoiceDetailDto?> ExecuteAsync(
            int id,
            CancellationToken cancellationToken = default
            ) 
        { 
            if(id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id),
                    "The invoice ID must be greater than zero"
                    );
            }

            Invoice? invoice = await _invoiceRepository.GetByIdAsync(
                id,
                cancellationToken
                );

            if (invoice is null)
            {
                return null;
            }

            return new InvoiceDetailDto(
                invoice.Id,
                invoice.Supplier.Name,
                invoice.InvoiceNumber,
                invoice.IssueDate,
                invoice.DueDate,
                invoice.TotalAmount,
                invoice.Currency,
                invoice.CustomerOrContactNumber,
                invoice.IsActive
                );
        }
    }
}
