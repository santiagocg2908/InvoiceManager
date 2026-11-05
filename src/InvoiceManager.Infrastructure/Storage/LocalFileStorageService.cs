using System.IO;
using InvoiceManager.Application.Abstractions.Storage;
using InvoiceManager.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InvoiceManager.Infrastructure.Storage
{
    public sealed class LocalFileStorageService : IFileStorageService
    {
        private const long MaxFileSizeBytes = 10L * 1024 * 1024;
        private const int BufferSize = 81920;

        private readonly string _rootPath;
        private readonly ILogger<LocalFileStorageService> _logger;

        public LocalFileStorageService(IOptions<FileStorageOptions> options, ILogger<LocalFileStorageService> logger)
        {
            _rootPath = Path.TrimEndingDirectorySeparator(
                Path.GetFullPath(options.Value.RootPath));
            _logger = logger;
        }

        public Task DeleteAsync(string storagePath,
            CancellationToken cancelToken = default)
        {
            cancelToken.ThrowIfCancellationRequested();

            string absoultePath = ResolveAbsolutePath(storagePath);
            if (File.Exists(absoultePath))
            {
                File.Delete(absoultePath);
                _logger.LogInformation(
                    "Document deleted: {StoragePath}",
                    storagePath
                    );
            }
            return Task.CompletedTask;
        }

        private string ResolveAbsolutePath(string storagePath)
        {
            if (string.IsNullOrWhiteSpace(storagePath) ||
                storagePath.Contains('\0'))
            {
                throw new ArgumentException(
                    "The storage path is not valid.",
                    nameof(storagePath));
            }

            string normalizedPath = storagePath
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);

            if (Path.IsPathRooted(normalizedPath) ||
                normalizedPath.Contains(':'))
            {
                throw new ArgumentException(
                    "Only allowed relative paths.",
                    nameof(storagePath));
            }

            string absolutePath = Path.GetFullPath(
                Path.Combine(_rootPath, normalizedPath)
            );

            string rootPrefix = _rootPath + Path.DirectorySeparatorChar;

            StringComparison comparison = OperatingSystem.IsWindows()
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            if (!absolutePath.StartsWith(rootPrefix, comparison))
            {
                throw new ArgumentException(
                    "The required Path is outside of allowed storage.",
                    nameof(storagePath));
            }

            return absolutePath;
        }

        public async Task<StoredFile> SaveAsync(
            Stream content,
            string originalFileName,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(content);

            if (!content.CanRead)
            {
                throw new InvalidInvoiceFileException(
                    "The document is can not be read"
                    );
            }

            string extension = GetExtensionFromContentType(contentType);

            string safeOriginalFileName = GetSafeFileName(originalFileName);

            string generatedFileName = $"{Guid.NewGuid():N}{extension}";

            DateTime uploadDate = DateTime.UtcNow;

            string year = uploadDate.ToString("yyyy");
            string month = uploadDate.ToString("MM");

            string relativeDirectory = Path.Combine(year,month);

            string absoluteDirectory = Path.Combine(
                _rootPath,
                relativeDirectory);

            Directory.CreateDirectory(absoluteDirectory);

            string absolutePath = Path.Combine(absoluteDirectory,generatedFileName);

            string storagePath = Path.Combine(
                relativeDirectory,
                generatedFileName
                ).Replace('\\','/');

            long totalBytes = 0;
            bool fileCreated = false;

            try
            {
                if (content.CanSeek) content.Position = 0;

                await using var destination = new FileStream(
                    absolutePath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    BufferSize,
                    FileOptions.Asynchronous | FileOptions.SequentialScan
                    );

                fileCreated = true;

                byte[] buffer = new byte[BufferSize];

                int bytesRead;

                while((bytesRead = await content.ReadAsync(
                    buffer.AsMemory(0,buffer.Length),
                    cancellationToken)) >0)
                {
                    totalBytes += bytesRead;

                    if(totalBytes > MaxFileSizeBytes)
                    {
                        throw new InvoiceFileTooLargeException(
                            "The file exceed the permitted limited of 10 MB"
                            );
                    }
                    await destination.WriteAsync(
                        buffer.AsMemory(0,bytesRead),
                        cancellationToken
                        );
                }

                if(totalBytes == 0)
                {
                    throw new InvalidInvoiceFileException(
                        "It can be saved a empty file"
                        );
                }

                await destination.FlushAsync(cancellationToken);

                _logger.LogInformation(
                    "The Document was saved correctly. Path: {StoragePath}",
                    storagePath
                    );

                return new StoredFile(
                    safeOriginalFileName,
                    contentType,
                    totalBytes,
                    storagePath
                    );
            }
            catch
            {
                if (fileCreated)
                {
                    try
                    {
                        if (File.Exists(absolutePath))
                        {
                            File.Delete(absolutePath);
                        }
                    }
                    catch (Exception cleanupException)
                    {
                        _logger.LogError(
                            cleanupException,
                            "It was not posible delete the parcial file: {StoragePath}",
                            storagePath
                            );
                    }
                }
                throw;
            }
        }

        private string GetSafeFileName(string originalFileName)
        {
            if (string.IsNullOrWhiteSpace(originalFileName))
            {
                throw new InvalidInvoiceFileException(
                    "The orinigal name of file is required"
                    );
            }

            string normalizeName = originalFileName.Replace('\\', '/');

            string safeFileName = Path.GetFileName(normalizeName);

            safeFileName = string.Concat(
                safeFileName.Where(character => !char.IsControl(character))
            );

            if (string.IsNullOrWhiteSpace(safeFileName))
            {
                throw new InvalidInvoiceFileException(
                    "The file name is not valid.");
            }

            if (safeFileName.Length > 255)
            {
                safeFileName = safeFileName[..255];
            }

            return safeFileName;
        }

        private string GetExtensionFromContentType(string contentType)
        {
            string normalizedContentType = contentType
                .Split(';',2)[0]
                .Trim()
                .ToLowerInvariant();
            return normalizedContentType switch
            {
                "application/pdf" => ".pdf",
                "image/png" => ".png",
                "image/jpeg" => ".jpg",

                _ => throw new UnsupportedInvoiceFileException(
                    "El tipo de documento no está permitido.")
            };
        }
    }
}
