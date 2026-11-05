using System.Globalization;

namespace InvoiceManager.Application.Abstractions.Storage
{
    public interface IFileStorageService
    {
        Task<StoredFile> SaveAsync(
            Stream content,
            string originalFileName,
            string contentType,
            CancellationToken cancellationToken = default
            );
        Task DeleteAsync(
            string storagePath,
            CancellationToken cancelToken = default
            );
    }
}
