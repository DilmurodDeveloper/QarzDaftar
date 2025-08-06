using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions
{
    public class AlreadyExistsUserPaymentLogException : Xeption
    {
        public AlreadyExistsUserPaymentLogException(Exception innerException)
            : base(message: "User payment log already exists.", innerException)
        { }
    }
}
