using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions
{
    public class PaymentDependencyException : Xeption
    {
        public PaymentDependencyException(Exception innerException)
            : base(message: "Payment dependency error occurred, contact support.",
                  innerException)
        { }
    }
}
