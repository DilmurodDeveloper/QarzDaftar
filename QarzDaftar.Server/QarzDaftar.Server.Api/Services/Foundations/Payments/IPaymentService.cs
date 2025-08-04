using QarzDaftar.Server.Api.Models.Foundations.Payments;

namespace QarzDaftar.Server.Api.Services.Foundations.Payments
{
    public interface IPaymentService
    {
        ValueTask<Payment> AddPaymentAsync(Payment payment);
        IQueryable<Payment> RetrieveAllPayments();
        ValueTask<Payment> RetrievePaymentByIdAsync(Guid paymentId);
        ValueTask<Payment> ModifyPaymentAsync(Payment payment);
    }
}
