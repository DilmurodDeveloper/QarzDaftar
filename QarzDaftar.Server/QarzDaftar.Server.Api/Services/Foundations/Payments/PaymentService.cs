using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.Payments;
using QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions;

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

        public async ValueTask<Payment> ModifyPaymentAsync(Payment payment)
        {
            try
            {
                ValidatePaymentOnModify(payment);

                Payment maybePayment =
                    await this.storageBroker.SelectPaymentByIdAsync(payment.Id);

                ValidateAgainstStoragePaymentOnModify(payment, maybePayment);

                return await this.storageBroker.UpdatePaymentAsync(payment);
            }
            catch (NullPaymentException nullPaymentException)
            {
                var paymentValidationException =
                    new PaymentValidationException(nullPaymentException);

                this.loggingBroker.LogError(paymentValidationException);

                throw paymentValidationException;
            }
            catch (InvalidPaymentException invalidPaymentException)
            {
                var paymentValidationException =
                    new PaymentValidationException(invalidPaymentException);

                this.loggingBroker.LogError(paymentValidationException);

                throw paymentValidationException;
            }
            catch (NotFoundPaymentException notFoundPaymentException)
            {
                var paymentValidationException =
                    new PaymentValidationException(notFoundPaymentException);

                this.loggingBroker.LogError(paymentValidationException);

                throw paymentValidationException;
            }
            catch (SqlException sqlException)
            {
                var failedPaymentStorageException =
                    new FailedPaymentStorageException(sqlException);

                var paymentDependencyException =
                    new PaymentDependencyException(failedPaymentStorageException);

                this.loggingBroker.LogCritical(paymentDependencyException);

                throw paymentDependencyException;
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedPaymentException =
                    new LockedPaymentException(dbUpdateConcurrencyException);

                var paymentDependencyValidationException =
                    new PaymentDependencyValidationException(lockedPaymentException);

                this.loggingBroker.LogError(paymentDependencyValidationException);

                throw paymentDependencyValidationException;
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedPaymentStorageException =
                    new FailedPaymentStorageException(dbUpdateException);

                var paymentDependencyException =
                    new PaymentDependencyException(failedPaymentStorageException);

                this.loggingBroker.LogError(paymentDependencyException);

                throw paymentDependencyException;
            }
            catch (Exception exception)
            {
                var failedPaymentServiceException =
                    new FailedPaymentServiceException(exception);

                var paymentServiceException =
                    new PaymentServiceException(failedPaymentServiceException);

                this.loggingBroker.LogError(paymentServiceException);

                throw paymentServiceException;
            }
        }
    }
}
