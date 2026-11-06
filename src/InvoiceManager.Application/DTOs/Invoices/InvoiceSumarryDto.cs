namespace InvoiceManager.Application.DTOs.Invoices;

public sealed record InvoiceSummaryDto(
    string SupplierName,
    string Currency,
    int TotalInvoices,
    decimal TotalAmount
);