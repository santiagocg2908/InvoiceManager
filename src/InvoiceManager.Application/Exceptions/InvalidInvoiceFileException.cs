namespace InvoiceManager.Application.Exceptions
{
    public sealed class InvalidInvoiceFileException: Exception
    {
        public InvalidInvoiceFileException(string message) : base(message)
        { }
    }
}
