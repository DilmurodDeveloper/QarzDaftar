using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions
{
    public class InvalidPaymentException : Xeption
    {
        public InvalidPaymentException()
            : base(message: "Payment is invalid.")
        { }
    }
}
