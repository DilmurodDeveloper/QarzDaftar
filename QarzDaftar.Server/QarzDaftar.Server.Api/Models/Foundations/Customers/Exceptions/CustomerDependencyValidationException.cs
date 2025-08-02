using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions
{
    public class CustomerDependencyValidationException : Xeption
    {
        public CustomerDependencyValidationException(Xeption innerException)
            : base(message: "Customer dependency validation error occurred, fix the errors and try again.",
                 innerException)
        { }
    }
}
