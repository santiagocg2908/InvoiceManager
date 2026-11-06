using System;
using System.Threading;
using System.Threading.Tasks;
using InvoiceManager.Application.Abstractions.Persistence;
using InvoiceManager.Application.DTOs.Invoices;
using InvoiceManager.Application.Exceptions;
using InvoiceManager.Domain.Entities;

namespace InvoiceManager.Application.UseCases.Invoices;

public sealed class UpdateInvoiceUseCase
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInvoiceUseCase(
        IInvoiceRepository invoiceRepository,
        ISupplierRepository supplierRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _supplierRepository = supplierRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InvoiceDetailDto?> ExecuteAsync(
        int id,
        UpdateInvoiceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(id),
                "The invoice ID must be greater than zero."
            );
        }

        Invoice? invoice =
            await _invoiceRepository.GetByIdForUpdateAsync(
                id,
                cancellationToken
            );

        if (invoice is null)
        {
            return null;
        }

        var supplierCandidate = new Supplier(
            request.SupplierName
        );

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

        invoice.UpdateDetails(
            supplier,
            request.InvoiceNumber,
            request.IssueDate,
            request.DueDate,
            request.TotalAmount,
            request.Currency,
            request.CustomerOrContactNumber
        );

        if (!isNewSupplier)
        {
            bool alreadyExists =
                await _invoiceRepository.ExistsForAnotherInvoiceAsync(
                    supplier.Id,
                    invoice.NormalizedInvoiceNumber,
                    invoice.Id,
                    cancellationToken
                );

            if (alreadyExists)
            {
                throw new DuplicateInvoiceException(
                    "Another invoice with that supplier and number already exists."
                );
            }
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken
        );

        return new InvoiceDetailDto(
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
}