using System.IO;
using InvoiceManager.Application.Abstractions.Extraction;
using InvoiceManager.Application.Abstractions.Persistence;
using InvoiceManager.Application.Abstractions.Storage;
using InvoiceManager.Application.DTOs.Invoices;
using InvoiceManager.Application.Exceptions;
using InvoiceManager.Application.Validations;
using InvoiceManager.Domain.Entities;

namespace InvoiceManager.Application.UseCases.Invoices
{
    public sealed class RegisterInvoicesUseCase
    {
        private readonly IInvoiceExtractionService _extractionService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterInvoicesUseCase(
            IInvoiceExtractionService extractionService,
            IFileStorageService fileStorageService,
            ISupplierRepository supplierRepository,
            IInvoiceRepository invoiceRepository,
            IUnitOfWork unitOfWork)
        {
            _extractionService = extractionService;
            _fileStorageService = fileStorageService;
            _supplierRepository = supplierRepository;
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task <RegisteredInvoiceResult> ExecuteAsync(
            ReguisterInvoiceRequest request,
            CancellationToken cancellationToken = default
            )
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.FileContent);

            string contentType = InvoiceFileValidator.ValidateMetaData(
                request.OriginalFileName,
                request.ContentType,
                request.FileSizeBytes
                );
            await using var fileBuffer = new MemoryStream();

            await CopyToBufferAsync(
                request.FileContent,
                fileBuffer,
                cancellationToken
                );
            if(fileBuffer.Length == 0)
            {
                throw new InvalidInvoiceFileException(
                    "The file is empty"
                    );
            }

            byte[] fileBytes = fileBuffer.ToArray();

            InvoiceFileValidator.ValidateSignature(
                fileBytes,
                contentType
                );

            fileBuffer.Position = 0;

            var extraction = await _extractionService.ExtractAsync(
                fileBuffer,
                contentType,
                cancellationToken
                );

            ValidateExtraction(extraction);

            var supplierCandidate = new Supplier(extraction.SupplierName);

            var supplier = await _supplierRepository
                .GetByNormalizedNameAsync(
                supplierCandidate.NormalizedName,
                cancellationToken
                );

            bool isNewSupplier = supplier is null;

            if (supplier is null)
            {
                supplier = supplierCandidate;
                _supplierRepository.Add(supplier);
            }

            var invoice = new Invoice(
                supplier,
                extraction.InvoiceNumber,
                extraction.IssueDate,
                extraction.TotalAmount,
                extraction.Currency,
                extraction.DueDate,
                extraction.CustomerOrContactNumber
                );

            if (!isNewSupplier)
            {
                bool alreadyExist = await _invoiceRepository.ExistAsync(
                    supplier.Id,
                    invoice.NormalizedInvoiceNumber,
                    cancellationToken
                    );

                if (alreadyExist) 
                {
                    throw new DuplicateInvoiceException(
                        "The invoice with that supplier and number it already exist"
                        );
                }
            }

            StoredFile? storedFile = null;

            try
            {
                fileBuffer.Position = 0;

                string safeFileName = Path.GetFileName(
                    request.OriginalFileName.Replace( "\\", "/" )
                    );

                storedFile = await _fileStorageService.SaveAsync(
                    fileBuffer,
                    safeFileName,
                    contentType,
                    cancellationToken
                    );

                _ = new InvoiceDocument(
                    invoice,
                    storedFile.OriginalFileName,
                    storedFile.ContentType,
                    storedFile.SizeBytes,
                    storedFile.StoragePath,
                    extraction.OriginalResponseJson
                    );

                _invoiceRepository.Add(invoice);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new RegisteredInvoiceResult(
                    invoice.Id,
                    invoice.Supplier.Name,
                    invoice.InvoiceNumber,
                    invoice.IssueDate,
                    invoice.DueDate,
                    invoice.TotalAmount,
                    invoice.Currency,
                    invoice.CustomerOrContactNumber,
                    invoice.IsActive
                    );
            }
            catch
            {
                //If the file has been saved but the persistence fail
                //try delete for avoid orphans files
                if(storedFile is not null)
                {
                    try
                    {
                        await _fileStorageService.DeleteAsync(
                            storedFile.StoragePath,
                            CancellationToken.None
                            );
                    }
                    catch
                    {

                    }
                }
                throw;
            }
        }

        private void ValidateExtraction(InvoiceExtractionResult extraction)
        {
            if(extraction is null)
            {
                throw new InvalidInvoiceExtractionException(
                    "The extraction service did not return any information"
                    );
            }
            if(string.IsNullOrWhiteSpace(extraction.SupplierName)||
                extraction.SupplierName.Length > 200)
            {
                throw new InvalidInvoiceExtractionException(
                    "The supplier name is not valid"
                    );
            }
            if(string.IsNullOrWhiteSpace(extraction.InvoiceNumber)||
                extraction.InvoiceNumber.Trim().Length> 100)
            {
                throw new InvalidInvoiceExtractionException(
                    "The invoice number is not valid"
                    );
            }

            if(extraction.IssueDate == default)
            {
                throw new InvalidInvoiceExtractionException(
                    "The emission date is not valid"
                    );
            }
            
            if(extraction.DueDate == default(DateOnly))
            {
                throw new InvalidInvoiceExtractionException(
                    "The Due Date is not valid"
                    );
            }

            if(extraction.TotalAmount <=0 ||
               extraction.TotalAmount > 9999999999999999.99m||
               decimal.Round(extraction.TotalAmount,2) != extraction.TotalAmount)
            {
                throw new InvalidInvoiceExtractionException(
                    "The invoice Amount is must be postive and maximun two decimals"
                    );
            }

            if (string.IsNullOrWhiteSpace(extraction.Currency))
            {
                throw new InvalidInvoiceExtractionException(
                    "The currency is required"
                    );
            }

            string currency = extraction.Currency.Trim().ToUpperInvariant();

            if(currency.Length !=3||
                currency.Any(character => character < 'A'|| character > 'Z'))
            {
                throw new InvalidInvoiceExtractionException(
                    "The currency is must be a 3 character code"
                    );
            }
            
            if(extraction.CustomerOrContactNumber?.Length > 100)
            {
                throw new InvalidInvoiceExtractionException(
                    "The Customer number o contract number is not valid"
                    );
            }

            if (string.IsNullOrWhiteSpace(extraction.OriginalResponseJson))
            {
                throw new InvalidInvoiceExtractionException(
                    "Not received the original model response"
                    );
            }
        }

        private async Task CopyToBufferAsync(
            Stream source, 
            MemoryStream destination, 
            CancellationToken cancellationToken)
        {
            if (!source.CanRead)
            {
                throw new InvalidInvoiceFileException(
                    "The document can not be read"
                    );
            }

            if (source.CanSeek)
            {
                source.Position = 0;
            }

            byte[] buffer = new byte[81920];

            long totalBytes = 0;
            int bytesRead;
            
            while((bytesRead = await source.ReadAsync(
                buffer.AsMemory(0, buffer.Length), cancellationToken))>0)
            {
                totalBytes += bytesRead;

                if(totalBytes > InvoiceFileValidator.MaxFileSizeBytes)
                {
                    throw new InvoiceFileTooLargeException(
                        "The file exceed the permitive limit of 10 MB"
                        );
                }

                await destination.WriteAsync(
                    buffer.AsMemory(0, bytesRead), 
                    cancellationToken);
            }
        }
        
    }
}
