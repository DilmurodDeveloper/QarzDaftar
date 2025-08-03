using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.Debts;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Debt> Debts { get; set; }

        public async ValueTask<Debt> InsertDebtAsync(Debt debt) =>
            await InsertAsync(debt);

        public IQueryable<Debt> SelectAllDebts()
        {
            var debts = SelectAll<Debt>()
                .Include(c => c.Customer);

            return debts;
        }

        public async ValueTask<Debt> SelectDebtByIdAsync(Guid debtId)
        {
            var debtWithDetails = Debts
                .Include(c => c.Customer)
                .FirstOrDefault(c => c.Id == debtId);

            return await ValueTask.FromResult(debtWithDetails);
        }

        public async ValueTask<Debt> UpdateDebtAsync(Debt debt) =>
            await UpdateAsync(debt);
    }
}
