using System.Globalization;

namespace InvoiceManager.Application.Exceptions
{
    public sealed class DuplicateInvoiceException: Exception
    {
        public DuplicateInvoiceException(string message):base(message)
        { 
        }
    }
}
