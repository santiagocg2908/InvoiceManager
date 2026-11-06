using System;

namespace InvoiceManager.Application.DTOs.Invoices
{
    public sealed class InvoiceListQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Supplier { get; init; }
        public DateOnly? IssueDateFrom { get; init; }
        public DateOnly? IssueDateTo { get; init; }
        public bool IncludeInactive { get; init; }=false;
    }
}
