using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogService
    {
        private static void ValidateUserPaymentLogNotNull(UserPaymentLog userPaymentLog)
        {
            if (userPaymentLog is null)
            {
                throw new NullUserPaymentLogException();
            }
        }
    }
}
