using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions
{
    public class FailedPaymentStorageException : Xeption
    {
        public FailedPaymentStorageException(Exception innerException)
            : base(message: "Failed payment storage error occurred, contact support.",
                  innerException)
        { }
    }
}
