using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions
{
    public class FailedCustomerServiceException : Xeption
    {
        public FailedCustomerServiceException(Exception innerException)
            : base(message: "Failed customer service error occurred, please contact support.",
                  innerException)
        { }
    }
}
