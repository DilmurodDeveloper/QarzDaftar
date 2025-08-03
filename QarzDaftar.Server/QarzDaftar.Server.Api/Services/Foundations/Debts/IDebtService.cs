using QarzDaftar.Server.Api.Models.Foundations.Debts;

namespace QarzDaftar.Server.Api.Services.Foundations.Debts
{
    public interface IDebtService
    {
        ValueTask<Debt> AddDebtAsync(Debt debt);
    }
}
