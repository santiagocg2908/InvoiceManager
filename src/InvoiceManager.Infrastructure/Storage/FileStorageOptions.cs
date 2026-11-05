using System;
namespace InvoiceManager.Infrastructure.Storage
{
    public sealed class FileStorageOptions
    {
        public const string SectionName = "FileStorage";

        public string RootPath { get; set; } = GetDefaultRootPath();

        private static string GetDefaultRootPath()
        {
            string basePath = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData
                );
            if (string.IsNullOrWhiteSpace(basePath))
            {
                basePath = Path.GetTempPath();
            }

            return Path.Combine(
                basePath,
                "InvoiceManager",
                "Documents"
                );
        }
    }
}
