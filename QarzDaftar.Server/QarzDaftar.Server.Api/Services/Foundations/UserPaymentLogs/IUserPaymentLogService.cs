using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;

namespace QarzDaftar.Server.Api.Services.Foundations.UserPaymentLogs
{
    public interface IUserPaymentLogService
    {
        ValueTask<UserPaymentLog> AddUserPaymentLogAsync(UserPaymentLog userPaymentLog);
        IQueryable<UserPaymentLog> RetrieveAllUserPaymentLogs();
        ValueTask<UserPaymentLog> RetrieveUserPaymentLogByIdAsync(Guid userPaymentLogId);
    }
}
