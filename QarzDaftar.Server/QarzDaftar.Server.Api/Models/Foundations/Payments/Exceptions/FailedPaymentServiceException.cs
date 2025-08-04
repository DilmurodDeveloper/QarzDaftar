using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions
{
    public class FailedPaymentServiceException : Xeption
    {
        public FailedPaymentServiceException(Exception innerException)
            : base(message: "Failed payment service error occurred, please contact support.",
                  innerException)
        { }
    }
}
