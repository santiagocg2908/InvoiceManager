namespace InvoiceManager.Application.Exceptions
{
    public sealed class InvoiceFileTooLargeException : Exception
    {
        public InvoiceFileTooLargeException(string message) : base(message) 
        { 
        }
    }
}
