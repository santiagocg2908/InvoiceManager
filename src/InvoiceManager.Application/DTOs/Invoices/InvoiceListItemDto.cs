using System;

namespace InvoiceManager.Application.DTOs.Invoices
{
    public  sealed record class InvoiceListItemDto(
        int Id,
        string SupplierName,
        string InvoiceNumber,
        DateOnly IssueDate,
        DateOnly? DueDate,
        decimal TotalAmount,
        string Currency,
        bool IsActive
        );
}
