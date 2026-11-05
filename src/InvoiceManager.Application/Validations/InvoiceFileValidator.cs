using System.IO;
using InvoiceManager.Application.Exceptions;

namespace InvoiceManager.Application.Validations
{
    public static class InvoiceFileValidator
    {
        public const long MaxFileSizeBytes = 10L * 1024 * 1024;

        public static string ValidateMetaData(
            string originalFileName,
            string contentType,
            long fileSizeBytes
            ) 
        {
            if (string.IsNullOrWhiteSpace(originalFileName))
            {
                throw new ArgumentNullException(
                    "The file name is required"
                    );
            }
            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ArgumentException(
                    "The content type is not specified"
                    );
            }
            if (fileSizeBytes <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "The file is empty"
                    );
            }
            if (fileSizeBytes > MaxFileSizeBytes)
            {
                throw new ArgumentException(
                    "The file exceed the permitted limit of 10 MB"
                    );
            }

            string documentExtension = Path.GetExtension(originalFileName)
                .ToLowerInvariant();

            string normalizedContentType = contentType.Split(';',2)[0].Trim().ToLowerInvariant();

            bool isSupported =
                (documentExtension == ".pdf" &&
                normalizedContentType == "application/pdf")
                ||
                (documentExtension == ".png" &&
                normalizedContentType == "image/png")
                ||
                ((documentExtension == ".jpg" || documentExtension == ".jpgeg")
                && normalizedContentType == "image/jpeg");

            if (!isSupported)
            {
                throw new ArgumentException(
                    "Only PDF, PNG and JPG files are allowed "
                    );
            }
            return normalizedContentType;
        }

        //For validate intial Signature
        public static void ValidateSignature(
            ReadOnlySpan<byte> content,
            string contentType
            )
        {
            bool isValid = contentType switch
            {
                "application/pdf" => HasPdfSignature(content),
                "image/png" => HasPngSignature(content),
                "image/jpeg" => HasJpegSignature(content),
                _ => false
            };
        }

        private static bool HasJpegSignature(ReadOnlySpan<byte> content)
        {
            return content.Length >= 3 &&
                content[0] == 0xFF &&
                content[1] == 0xD8 &&
                content[2] == 0xFF;

        }

        private static bool HasPngSignature(ReadOnlySpan<byte> content)
        {
            byte[] signature =
                [
                    0x89, 0x50, 0x4E, 0x47,
                    0x0D, 0x0A, 0x1A, 0x0A
                ];
            return content.Length >= signature.Length && 
                content[..signature.Length].SequenceEqual(signature);
        }

        private static bool HasPdfSignature(ReadOnlySpan<byte> content)
        {
            return content.Length >= 5 &&
                content[0] == 0x25 &&
                content[1] == 0x50 &&
                content[2] == 0x44 &&
                content[3] == 0x46 &&
                content[4] == 0x2D;
        }
    }
}
