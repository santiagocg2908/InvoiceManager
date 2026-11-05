namespace InvoiceManager.Application.Exceptions
{
    public sealed class DuplicateSupplierException:Exception
    {
        public DuplicateSupplierException(
            string message,
            Exception? innerException = null ) 
            : base( message, innerException ) 
        {
        }
    }
}
