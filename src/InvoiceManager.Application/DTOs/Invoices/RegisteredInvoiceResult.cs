namespace InvoiceManager.Application.DTOs.Invoices
{
    public sealed record RegisteredInvoiceResult(
        int Id,
        string SupplierName,
        string InvoiceNumber,
        DateOnly IssueDate,
        DateOnly? DueDate,
        decimal TotalAmount,
        string Currency,
        string? CustomerOrContactNumber,
        bool IsActive
        );
}
