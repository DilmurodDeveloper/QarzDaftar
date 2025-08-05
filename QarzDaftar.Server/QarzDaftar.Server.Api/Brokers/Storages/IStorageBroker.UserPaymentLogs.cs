using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<UserPaymentLog> InsertUserPaymentLogAsync(UserPaymentLog userPaymentLog);
    }
}
