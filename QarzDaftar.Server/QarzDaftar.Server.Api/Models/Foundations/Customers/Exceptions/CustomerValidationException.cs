using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions
{
    public class CustomerValidationException : Xeption
    {
        public CustomerValidationException(Xeption innerException)
            : base(message: "Customer validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
