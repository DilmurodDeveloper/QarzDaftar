using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.Payments;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Payment> Payments { get; set; }

        public async ValueTask<Payment> InsertPaymentAsync(Payment payment) =>
            await InsertAsync(payment);

        public IQueryable<Payment> SelectAllPayments()
        {
            var payments = SelectAll<Payment>()
                .Include(c => c.Customer);

            return payments;
        }

        public async ValueTask<Payment> SelectPaymentByIdAsync(Guid paymentId)
        {
            var paymentWithDetails = Payments
                .Include(c => c.Customer)
                .FirstOrDefault(c => c.Id == paymentId);

            return await ValueTask.FromResult(paymentWithDetails);
        }

        public async ValueTask<Payment> UpdatePaymentAsync(Payment payment) =>
            await UpdateAsync(payment);

        public async ValueTask<Payment> DeletePaymentAsync(Payment payment) =>
            await UpdateAsync(payment);
    }
}
