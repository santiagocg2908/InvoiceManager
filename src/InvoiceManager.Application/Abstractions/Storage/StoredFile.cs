namespace InvoiceManager.Application.Abstractions.Storage
{
    public sealed record StoredFile(
        string OriginalFileName,
        string ContentType,
        long SizeBytes,
        string StoragePath
        );
}
