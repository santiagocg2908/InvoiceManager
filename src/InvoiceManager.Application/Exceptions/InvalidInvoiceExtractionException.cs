using System.Reflection;

namespace InvoiceManager.Application.Exceptions
{
    public sealed class InvalidInvoiceExtractionException: Exception
    {
        public InvalidInvoiceExtractionException(string message): base(message) 
        { 
        }
    }
}
