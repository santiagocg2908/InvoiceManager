using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Security;
using System.Text;
using System.Xml.XPath;

namespace InvoiceManager.Domain.Entities
{
    public class Invoice
    {
        private Invoice() 
        { 
        }
        public Invoice(
            Supplier supplier,
            string invoiceNumber,
            DateOnly issueDate,
            decimal totalAmount,
            string currency,//codigo de moneda
            DateOnly? dueDate = null, //fecha de vencimiento
            string? customerOrContractNumber = null
            )
        {
            ArgumentNullException.ThrowIfNull(supplier);
            Supplier = supplier;
            SupplierId = supplier.Id;

            SetInvoiceNumber(invoiceNumber);

            if(issueDate == default)
            {
                throw new ArgumentNullException("The emission date is required. ",
                    nameof(issueDate));

            }
            if (dueDate == default(DateOnly)) 
            {
                throw new ArgumentException("The Due Date is not valid. ",
                    nameof(issueDate));
            }
            if (totalAmount <= 0) 
            {
                throw new ArgumentException("The total amount should be more than zero",
                    nameof(totalAmount));
            }
            IssueDate = issueDate;
            DueDate = dueDate;
            TotalAmount = totalAmount;

            SetCurrency(currency);

            CustomerOrContactNumber = string.IsNullOrWhiteSpace(customerOrContractNumber)
                ? null : customerOrContractNumber.Trim();
            IsActive = true;
            CreatedAtUtc = DateTime.UtcNow;

        }
        public int Id {  get; private set; }
        public int SupplierId { get; private set; }
        public Supplier Supplier { get; private set; }
        public string InvoiceNumber {  get; private set; } = string.Empty;
        public string NormalizedInvoiceNumber { get; private set; } = string.Empty;
        public DateOnly IssueDate { get; private set; }
        public DateOnly? DueDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string Currency { get; private set; } = string.Empty;
        public string? CustomerOrContactNumber {  get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAtUtc {  get; private set; }
        public InvoiceDocument? Document { get; private set; }

        public void CorrectExtractedData(
            string invoiceNumber,
            DateOnly issueDate,
            decimal totalAmount,
            string currency,
            DateOnly? dueDate,
            string? customerOrContractNumber
            )
        {
            SetInvoiceNumber( invoiceNumber );
            if (issueDate == default)
            {
                throw new ArgumentException( "The Issue Date is not valid",
                    nameof(issueDate)
                    );
            }
            if (dueDate == default(DateOnly))
            {
                throw new ArgumentException(
                    "The Due Date is not valid"
                    );
            }
            if (totalAmount<=0)
            {
                throw new ArgumentException(
                    "The Total Amount should be more than zero"
                    );
            }
            IssueDate = issueDate;
            DueDate = dueDate;
            TotalAmount = totalAmount;
            SetCurrency( currency );
            CustomerOrContactNumber = string.IsNullOrWhiteSpace(customerOrContractNumber)
                ? null : customerOrContractNumber.Trim();
        }

        public void ChangeSupplier(Supplier supplier)
        {
            ArgumentNullException.ThrowIfNull(supplier);
            Supplier = supplier;
            SupplierId = supplier.Id;
        }

        public void Desactive()
        {
            IsActive = false;
        }

        public void AttachDocument(InvoiceDocument document) 
        {
            ArgumentNullException.ThrowIfNull(document);

            if(Document is not null)
            {
                throw new InvalidOperationException("The invoice al ready had a document");
            }

            if(document.Invoice != this)
            {
                throw new InvalidOperationException("The document is belongs to another invoice");
            }
            Document = document;
        }

        private void SetInvoiceNumber(string invoiceNumber)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber))
            {
                throw new ArgumentException(
                    "The Invoice Number is required",
                    nameof(invoiceNumber));
            }

            if (invoiceNumber.Trim().Length > 100)
            {
                throw new ArgumentException(
                    "The Invoice Number can not exceed the 100 characters",
                    nameof(invoiceNumber));
            }
            InvoiceNumber = invoiceNumber;
            NormalizedInvoiceNumber = InvoiceNumber.ToUpperInvariant();
        }
        private void SetCurrency(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentNullException("The currency is required",
                    nameof(currency));
            }

            string normalizedCurrency = currency.Trim().ToUpperInvariant();

            if (normalizedCurrency.Length != 3 || normalizedCurrency.Any(c=>c< 'A' || c > 'Z'))
            {
                throw new ArgumentException("The currenc should be a 3 letters code",
                    nameof(currency));
            }
            Currency = normalizedCurrency;
        }
    }
}
