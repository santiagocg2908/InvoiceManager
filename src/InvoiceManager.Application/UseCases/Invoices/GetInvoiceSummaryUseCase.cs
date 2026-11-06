using InvoiceManager.Application.Abstractions.Persistence;
using InvoiceManager.Application.DTOs.Invoices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InvoiceManager.Application.UseCases.Invoices;

public sealed class GetInvoiceSummaryUseCase
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceSummaryUseCase(
        IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public Task<IReadOnlyList<InvoiceSummaryDto>> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        return _invoiceRepository.GetSummaryAsync(
            cancellationToken
        );
    }
}