using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using QarzDaftar.Server.Api.Models.Foundations.Payments;
using QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions;
using Xeptions;

namespace QarzDaftar.Server.Api.Services.Foundations.Payments
{
    public partial class PaymentService
    {
        private delegate ValueTask<Payment> ReturningPaymentFunction();
        private delegate IQueryable<Payment> ReturningPaymentsFunction();

        private async ValueTask<Payment> TryCatch(ReturningPaymentFunction returningPaymentFunction)
        {
            try
            {
                return await returningPaymentFunction();
            }
            catch (NullPaymentException nullPaymentException)
            {
                throw CreateAndLogValidationException(nullPaymentException);
            }
            catch (InvalidPaymentException invalidPaymentException)
            {
                throw CreateAndLogValidationException(invalidPaymentException);
            }
            catch (NotFoundPaymentException notFoundPaymentException)
            {
                throw CreateAndLogValidationException(notFoundPaymentException);
            }
            catch (SqlException sqlException)
            {
                var failedPaymentStorageException =
                    new FailedPaymentStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedPaymentStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsPaymentException =
                    new AlreadyExistsPaymentException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsPaymentException);
            }
            catch (Exception exception)
            {
                var failedPaymentServiceException =
                    new FailedPaymentServiceException(exception);

                throw CreateAndLogServiceException(failedPaymentServiceException);
            }
        }

        private IQueryable<Payment> TryCatch(ReturningPaymentsFunction returningPaymentsFunction)
        {
            try
            {
                return returningPaymentsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedPaymentStorageException =
                    new FailedPaymentStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedPaymentStorageException);
            }
            catch (Exception exception)
            {
                var failedPaymentServiceException =
                    new FailedPaymentServiceException(exception);

                throw CreateAndLogServiceException(failedPaymentServiceException);
            }
        }

        private PaymentValidationException CreateAndLogValidationException(Xeption exception)
        {
            var PaymentValidationException = new PaymentValidationException(exception);
            this.loggingBroker.LogError(PaymentValidationException);

            return PaymentValidationException;
        }

        private PaymentDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var PaymentDependencyException = new PaymentDependencyException(exception);
            this.loggingBroker.LogCritical(PaymentDependencyException);

            return PaymentDependencyException;
        }

        private PaymentDependencyValidationException CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var PaymentDependencyValidationException =
                new PaymentDependencyValidationException(exception);

            this.loggingBroker.LogError(PaymentDependencyValidationException);

            return PaymentDependencyValidationException;
        }

        private PaymentServiceException CreateAndLogServiceException(Xeption exception)
        {
            var PaymentServiceException = new PaymentServiceException(exception);
            this.loggingBroker.LogError(PaymentServiceException);

            return PaymentServiceException;
        }
    }
}
