using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions
{
    public class CustomerDependencyException : Xeption
    {
        public CustomerDependencyException(Exception innerException)
            : base(message: "Customer dependency error occurred, contact support.",
                  innerException)
        { }
    }
}
