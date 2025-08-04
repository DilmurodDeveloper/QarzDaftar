using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions
{
    public class PaymentServiceException : Xeption
    {
        public PaymentServiceException(Exception innerException)
            : base(message: "Payment service error occurred, contact support.",
                  innerException)
        { }
    }
}
