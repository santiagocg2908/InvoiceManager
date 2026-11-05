using System;
using System.Collections.Generic;
using System.Text;

namespace InvoiceManager.Domain.Entities
{
    public class InvoiceDocument
    {
        private InvoiceDocument() 
        { 
        }
        public InvoiceDocument(
            Invoice invoice,
            string originalFileName,
            string contentType,
            long sizeBytes,
            string storagePath,
            string originalLlmResponseJson
            )
        {
            ArgumentNullException.ThrowIfNull( invoice );

            if (string.IsNullOrWhiteSpace(originalFileName))
            {
                throw new ArgumentException(
                    "The file name is required",
                    nameof(originalFileName));
            }
            if(sizeBytes <= 0)
            {
                throw new ArgumentException(
                    "The file size it must be more than zero"
                    );
            }
            if (string.IsNullOrWhiteSpace(storagePath)) 
            {
                throw new ArgumentException(
                    "The storage path is required",
                    nameof(storagePath)
                    );
            }
            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ArgumentException(
                    "The content type is required",
                    nameof(contentType)
                    );
            }
            if (string.IsNullOrWhiteSpace(originalLlmResponseJson))
            {
                throw new ArgumentException(
                    "The original LLM response is required",
                    nameof(originalLlmResponseJson)
                    );
            }
            Invoice = invoice;
            InvoiceId= invoice.Id;

            OriginalFileName = originalFileName.Trim();
            ContentType = contentType;
            SizeBytes = sizeBytes;
            StoragePath = storagePath;
            OriginalLlmResponseJson = originalLlmResponseJson;

            UploadedAtUtc = DateTime.UtcNow;
            invoice.AttachDocument(this);
        }
        public int Id { get; private set; }
        public int InvoiceId { get; private set; }
        public Invoice Invoice { get; private set; } = null!;
        public string OriginalFileName { get; private set; } = string.Empty;
        public string ContentType { get; private set; } = string.Empty;
        public long SizeBytes { get; private set; }
        public DateTime UploadedAtUtc { get; private set; }
        public string StoragePath { get; private set; } = string.Empty;
        public string OriginalLlmResponseJson { get; private set; } = string.Empty;
    }
}
