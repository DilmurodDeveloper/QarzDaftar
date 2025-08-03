using QarzDaftar.Server.Api.Models.Foundations.Debts;

namespace QarzDaftar.Server.Api.Services.Foundations.Debts
{
    public interface IDebtService
    {
        ValueTask<Debt> AddDebtAsync(Debt debt);
        IQueryable<Debt> RetrieveAllDebts();
        ValueTask<Debt> RetrieveDebtByIdAsync(Guid debtId);
        ValueTask<Debt> ModifyDebtAsync(Debt debt);
        ValueTask<Debt> RemoveDebtByIdAsync(Guid debtId);
    }
}
