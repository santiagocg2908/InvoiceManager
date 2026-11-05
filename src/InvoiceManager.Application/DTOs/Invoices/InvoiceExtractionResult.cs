namespace InvoiceManager.Application.DTOs.Invoices
{
    public sealed record InvoiceExtractionResult(
        string SupplierName,
        string InvoiceNumber,
        DateOnly IssueDate,
        DateOnly? DueDate,
        decimal TotalAmount,
        string Currency,
        string? CustomerOrContactNumber,
        string OriginalResponseJson
        );
}
