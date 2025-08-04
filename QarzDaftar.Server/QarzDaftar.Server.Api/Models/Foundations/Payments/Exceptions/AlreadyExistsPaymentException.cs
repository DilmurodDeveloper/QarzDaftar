using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions
{
    public class AlreadyExistsPaymentException : Xeption
    {
        public AlreadyExistsPaymentException(Exception innerException)
            : base(message: "Payment already exists.", innerException)
        { }
    }
}
