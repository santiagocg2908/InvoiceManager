using InvoiceManager.Application.DTOs.Invoices;
using System.Globalization;

namespace InvoiceManager.Application.Abstractions.Extraction
{
    //LLM Interface Service
    public interface IInvoiceExtractionService
    {
        Task<InvoiceExtractionResult> ExtractAsync(
            Stream document,
            string contentType,
            CancellationToken cancellationToken = default
            );
    }
}
