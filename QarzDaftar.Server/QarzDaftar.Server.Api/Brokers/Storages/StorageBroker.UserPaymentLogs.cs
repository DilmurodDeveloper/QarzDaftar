using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<UserPaymentLog> UserPaymentLogs { get; set; }

        public async ValueTask<UserPaymentLog> InsertUserPaymentLogAsync(
            UserPaymentLog userPaymentLog) => await InsertAsync(userPaymentLog);

        public IQueryable<UserPaymentLog> SelectAllUserPaymentLogs()
        {
            var userPaymentLogs = SelectAll<UserPaymentLog>()
                .Include(c => c.User);

            return userPaymentLogs;
        }

        public async ValueTask<UserPaymentLog> SelectUserPaymentLogByIdAsync(Guid userPaymentLogId)
        {
            var userPaymentLogWithDetails = UserPaymentLogs
                .Include(c => c.User)
                .FirstOrDefault(c => c.Id == userPaymentLogId);

            return await ValueTask.FromResult(userPaymentLogWithDetails);
        }

        public async ValueTask<UserPaymentLog> UpdateUserPaymentLogAsync(
            UserPaymentLog userPaymentLog) => await UpdateAsync(userPaymentLog);
    }
}
