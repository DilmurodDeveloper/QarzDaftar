using QarzDaftar.Server.Api.Models.Foundations.Debts;

namespace QarzDaftar.Server.Api.Services.Processings.Debts
{
    public interface IDebtProcessingService
    {
        ValueTask<Debt> AddDebtAsync(Debt debt, Guid userId);
        IQueryable<Debt> RetrieveAllDebtsByUserId(Guid userId);
        ValueTask<Debt> RetrieveDebtByIdAsync(Guid debtId);
        IQueryable<Debt> RetrieveAllDebtsByCustomerId(Guid customerId, Guid userId);
        ValueTask<Debt> ModifyDebtAsync(Debt debt, Guid userId);
        ValueTask<Debt> RemoveDebtByIdAsync(Guid debtId, Guid userId);
        decimal CalculateTotalDebtForCustomer(Guid customerId);
        decimal CalculateTotalDebtByUserId(Guid userId);
    }
}
