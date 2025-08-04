using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions
{
    public class PaymentValidationException : Xeption
    {
        public PaymentValidationException(Xeption innerException)
            : base(message: "Payment validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
