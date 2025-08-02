using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.Users;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<User> Users { get; set; }

        public async ValueTask<User> InsertUserAsync(User user) =>
            await InsertAsync(user);

        public IQueryable<User> SelectAllUsers()
        {
            var users = SelectAll<User>()
                .Include(a => a.Customers)
                .Include(a => a.Debts)
                .Include(a => a.UserNotes)
                .Include(a => a.SubscriptionHistories)
                .Include(a => a.PaymentLogs);

            return users;
        }

        public async ValueTask<User> SelectUserByIdAsync(Guid userId)
        {
            var userWithDetails = Users
                .Include(user => user.Customers)
                .Include(user => user.Debts)
                .Include(user => user.UserNotes)
                .Include(user => user.SubscriptionHistories)
                .Include(user => user.PaymentLogs)
                .FirstOrDefault(c => c.Id == userId);

            return await ValueTask.FromResult(userWithDetails);
        }

        public async ValueTask<User> UpdateUserAsync(User user) =>
            await UpdateAsync(user);

        public async ValueTask<User> DeleteUserAsync(User user) =>
            await DeleteAsync(user);
    }
}
