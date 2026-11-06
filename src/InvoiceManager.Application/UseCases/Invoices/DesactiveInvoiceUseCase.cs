using System;
using System.Threading;
using System.Threading.Tasks;
using InvoiceManager.Application.Abstractions.Persistence;
using InvoiceManager.Domain.Entities;

namespace InvoiceManager.Application.UseCases.Invoices
{
    public sealed class DesactiveInvoiceUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DesactiveInvoiceUseCase(
            IInvoiceRepository invoiceRepository,
            IUnitOfWork unitOfWork
            )
        {
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ExecuteAsync(
            int id,
            CancellationToken cancellationToken = default
            )
        {
            if(id <= 0)
            {
                throw new ArgumentException(
                    "The invoice Id must be greaten than zero"
                    );
            }

            Invoice? invoice = 
                await _invoiceRepository.GetByIdForUpdateAsync( id,
                cancellationToken );

            if (invoice is null) 
            { 
                return false;
            }

            invoice.Desactive();

            await _unitOfWork.SaveChangesAsync(
                cancellationToken
                );

            return true;
        }
    }
}
