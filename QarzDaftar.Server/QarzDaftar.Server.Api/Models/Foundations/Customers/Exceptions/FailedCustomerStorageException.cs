using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions
{
    public class FailedCustomerStorageException : Xeption
    {
        public FailedCustomerStorageException(Exception innerException)
            : base(message: "Failed customer storage error occurred, contact support.",
                  innerException)
        { }
    }
}
