using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions
{
    public class CustomerServiceException : Xeption
    {
        public CustomerServiceException(Exception innerException)
            : base(message: "Customer service error occurred, contact support.",
                  innerException)
        { }
    }
}
