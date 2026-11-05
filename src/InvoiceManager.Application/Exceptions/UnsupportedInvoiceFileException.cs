using System.Reflection;

namespace InvoiceManager.Application.Exceptions
{
    public sealed class UnsupportedInvoiceFileException : Exception
    {
        public UnsupportedInvoiceFileException(string message):base(message) 
        { 
        }
    }
}
