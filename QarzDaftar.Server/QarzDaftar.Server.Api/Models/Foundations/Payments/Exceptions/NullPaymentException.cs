using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions
{
    public class NullPaymentException : Xeption
    {
        public NullPaymentException()
            : base(message: "Payment is null.")
        { }
    }
}
