using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions
{
    public class InvalidUserPaymentLogException : Xeption
    {
        public InvalidUserPaymentLogException()
            : base(message: "User payment log is invalid.")
        { }
    }
}
