using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.Payments;

namespace QarzDaftar.Server.Api.Services.Foundations.Payments
{
    public partial class PaymentService : IPaymentService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public PaymentService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<Payment> AddPaymentAsync(Payment payment) =>
        TryCatch(async () =>
        {
            ValidatePaymentOnAdd(payment);

            return await this.storageBroker.InsertPaymentAsync(payment);
        });

        public IQueryable<Payment> RetrieveAllPayments() =>
            TryCatch(() => this.storageBroker.SelectAllPayments());

        public ValueTask<Payment> RetrievePaymentByIdAsync(Guid paymentId) =>
        TryCatch(async () =>
        {
            ValidatePaymentId(paymentId);

            Payment maybePayment =
                await this.storageBroker.SelectPaymentByIdAsync(paymentId);

            ValidateStoragePayment(maybePayment, paymentId);

            return maybePayment;
        });

        public ValueTask<Payment> ModifyPaymentAsync(Payment payment) =>
        TryCatch(async () =>
        {
            ValidatePaymentOnModify(payment);

            Payment maybePayment =
                await this.storageBroker.SelectPaymentByIdAsync(payment.Id);

            ValidateAgainstStoragePaymentOnModify(payment, maybePayment);

            return await this.storageBroker.UpdatePaymentAsync(payment);
        });

        public async ValueTask<Payment> RemovePaymentByIdAsync(Guid paymentId)
        {
            Payment maybePayment =
                await this.storageBroker.SelectPaymentByIdAsync(paymentId);

            return await this.storageBroker.DeletePaymentAsync(maybePayment);
        }
    }
}
