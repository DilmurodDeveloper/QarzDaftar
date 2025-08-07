using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Services.Foundations.Customers;
using QarzDaftar.Server.Api.Services.Foundations.Debts;

namespace QarzDaftar.Server.Api.Services.Processings.Debts
{
    public partial class DebtProcessingService : IDebtProcessingService
    {
        private readonly IDebtService debtService;
        private readonly ICustomerService customerService;

        public DebtProcessingService(
            IDebtService debtService,
            ICustomerService customerService)
        {
            this.debtService = debtService;
            this.customerService = customerService;
        }

        public async ValueTask<Debt> AddDebtAsync(Debt debt, Guid userId)
        {
            Customer customer = await customerService.RetrieveCustomerByIdAsync(debt.CustomerId);

            if (customer.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to add debt to this customer.");

            debt.Id = Guid.NewGuid();
            debt.RemainingAmount = debt.Amount;
            debt.Status = DebtStatus.Unpaid;
            debt.CreatedDate = DateTimeOffset.UtcNow;
            debt.UpdatedDate = DateTimeOffset.UtcNow;

            return await debtService.AddDebtAsync(debt);
        }

        public IQueryable<Debt> RetrieveAllDebtsByUserId(Guid userId) =>
            debtService.RetrieveAllDebts()
                .Where(debt => debt.Customer!.UserId == userId);

        public async ValueTask<Debt> RetrieveDebtByIdAsync(Guid debtId) =>
            await debtService.RetrieveDebtByIdAsync(debtId);

        public IQueryable<Debt> RetrieveAllDebtsByCustomerId(Guid customerId, Guid userId)
        {
            Customer customer = customerService.RetrieveAllCustomers(userId)
                .FirstOrDefault(c => c.Id == customerId && c.UserId == userId)
                ?? throw new UnauthorizedAccessException("You are not authorized to view this customer's debts.");

            return debtService.RetrieveAllDebtsByCustomerId(customerId);
        }

        public async ValueTask<Debt> ModifyDebtAsync(Debt debt, Guid userId)
        {
            Customer customer = await customerService.RetrieveCustomerByIdAsync(debt.CustomerId);

            if (customer.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to modify this debt.");

            debt.UpdatedDate = DateTimeOffset.UtcNow;

            return await debtService.ModifyDebtAsync(debt);
        }

        public async ValueTask<Debt> RemoveDebtByIdAsync(Guid debtId, Guid userId)
        {
            Debt debt = await debtService.RetrieveDebtByIdAsync(debtId);

            Customer customer = await customerService.RetrieveCustomerByIdAsync(debt.CustomerId);

            if (customer.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to delete this debt.");

            return await debtService.RemoveDebtByIdAsync(debtId);
        }

        public decimal CalculateTotalDebtForCustomer(Guid customerId)
        {
            var debts = debtService.RetrieveAllDebtsByCustomerId(customerId);
            return debts.Sum(d => d.RemainingAmount);
        }

        public decimal CalculateTotalDebtByUserId(Guid userId)
        {
            var debts = debtService.RetrieveAllDebts()
                .Where(debt => debt.Customer.UserId == userId);

            return debts.Sum(debt => debt.Amount);
        }
    }
}
