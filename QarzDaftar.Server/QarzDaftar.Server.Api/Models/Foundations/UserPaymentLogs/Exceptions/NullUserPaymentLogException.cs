using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions
{
    public class NullUserPaymentLogException : Xeption
    {
        public NullUserPaymentLogException()
            : base(message: "User payment log is null.")
        { }
    }
}
