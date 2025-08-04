using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions
{
    public class NotFoundPaymentException : Xeption
    {
        public NotFoundPaymentException(Guid paymentId)
            : base(message: $"Payment is not found with id: {paymentId}")
        { }
    }
}
