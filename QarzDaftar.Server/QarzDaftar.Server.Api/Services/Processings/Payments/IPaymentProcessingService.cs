using QarzDaftar.Server.Api.Models.Foundations.Payments;

namespace QarzDaftar.Server.Api.Services.Processings.Payments
{
    public interface IPaymentProcessingService
    {
        ValueTask<Payment> AddPaymentAsync(Payment payment, Guid userId);
        IQueryable<Payment> RetrieveAllPaymentsByUserId(Guid userId);
        ValueTask<Payment> RetrievePaymentByIdAsync(Guid paymentId);
        ValueTask<Payment> ModifyPaymentAsync(Payment payment, Guid userId);
        ValueTask<Payment> RemovePaymentByIdAsync(Guid paymentId, Guid userId);
        decimal CalculateTotalPaidAmountForCustomer(Guid customerId);
        decimal CalculateRemainingDebtForCustomer(Guid customerId);
        decimal CalculateTotalPaidByUserId(Guid userId);
        decimal CalculateRemainingDebtByUserId(Guid userId);
    }
}
