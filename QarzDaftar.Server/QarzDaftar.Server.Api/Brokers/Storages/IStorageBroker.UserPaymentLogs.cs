using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<UserPaymentLog> InsertUserPaymentLogAsync(UserPaymentLog userPaymentLog);
        IQueryable<UserPaymentLog> SelectAllUserPaymentLogs();
        ValueTask<UserPaymentLog> SelectUserPaymentLogByIdAsync(Guid userPaymentLogId);
        ValueTask<UserPaymentLog> UpdateUserPaymentLogAsync(UserPaymentLog userPaymentLog);
        ValueTask<UserPaymentLog> DeleteUserPaymentLogAsync(UserPaymentLog userPaymentLog);
    }
}
