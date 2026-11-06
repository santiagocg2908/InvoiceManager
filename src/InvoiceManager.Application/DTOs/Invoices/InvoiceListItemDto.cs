using System;

namespace InvoiceManager.Application.DTOs.Invoices
{
    public  sealed record class InvoiceListItemDto(
        int Id,
        string SupplierName,
        string InvoiceNumber,
        DateOnly IssueDate,
        DateOnly? DueDate,
        decimal TotalaAmount,
        string Currency,
        bool IsActive
        );
}
