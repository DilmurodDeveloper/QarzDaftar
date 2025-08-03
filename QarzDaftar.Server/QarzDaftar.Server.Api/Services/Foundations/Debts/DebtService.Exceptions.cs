using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;
using Xeptions;

namespace QarzDaftar.Server.Api.Services.Foundations.Debts
{
    public partial class DebtService
    {
        private delegate ValueTask<Debt> ReturningDebtFunction();

        private async ValueTask<Debt> TryCatch(ReturningDebtFunction returningDebtFunction)
        {
            try
            {
                return await returningDebtFunction();
            }
            catch (NullDebtException nullDebtException)
            {
                throw CreateAndLogValidationException(nullDebtException);
            }
            catch (InvalidDebtException invalidDebtException)
            {
                throw CreateAndLogValidationException(invalidDebtException);
            }
            catch (SqlException sqlException)
            {
                var failedDebtStorageException =
                    new FailedDebtStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedDebtStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsDebtException =
                    new AlreadyExistsDebtException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsDebtException);
            }
            catch (Exception exception)
            {
                var failedDebtServiceException =
                    new FailedDebtServiceException(exception);

                throw CreateAndLogServiceException(failedDebtServiceException);
            }
        }

        private DebtValidationException CreateAndLogValidationException(Xeption exception)
        {
            var DebtValidationException = new DebtValidationException(exception);
            this.loggingBroker.LogError(DebtValidationException);

            return DebtValidationException;
        }

        private DebtDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var DebtDependencyException = new DebtDependencyException(exception);
            this.loggingBroker.LogCritical(DebtDependencyException);

            return DebtDependencyException;
        }

        private DebtDependencyValidationException CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var DebtDependencyValidationException =
                new DebtDependencyValidationException(exception);

            this.loggingBroker.LogError(DebtDependencyValidationException);

            return DebtDependencyValidationException;
        }

        private DebtServiceException CreateAndLogServiceException(Xeption exception)
        {
            var DebtServiceException = new DebtServiceException(exception);
            this.loggingBroker.LogError(DebtServiceException);

            return DebtServiceException;
        }
    }
}
