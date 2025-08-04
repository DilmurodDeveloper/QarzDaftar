using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions
{
    public class PaymentDependencyValidationException : Xeption
    {
        public PaymentDependencyValidationException(Xeption innerException)
            : base(message: "Payment dependency validation error occurred, fix the errors and try again.",
                 innerException)
        { }
    }
}
