using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions
{
    public class LockedPaymentException : Xeption
    {
        public LockedPaymentException(Exception innerException)
            : base(message: "Payment is locked, try again later.",
                  innerException)
        { }
    }
}
