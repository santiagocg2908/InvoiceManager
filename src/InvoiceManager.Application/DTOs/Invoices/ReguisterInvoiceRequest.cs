namespace InvoiceManager.Application.DTOs.Invoices
{
    public sealed record ReguisterInvoiceRequest(
        Stream FileContent,
        string OriginalFileName,
        string ContentType,
        long FileSizeBytes
        );
}
