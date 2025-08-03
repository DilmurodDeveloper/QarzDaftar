using QarzDaftar.Server.Api.Models.Foundations.Debts;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Debt> InsertDebtAsync(Debt debt);
    }
}
