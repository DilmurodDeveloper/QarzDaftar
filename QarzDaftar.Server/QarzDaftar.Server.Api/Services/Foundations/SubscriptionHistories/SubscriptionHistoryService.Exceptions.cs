using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions;
using Xeptions;

namespace QarzDaftar.Server.Api.Services.Foundations.SubscriptionHistories
{
    public partial class SubscriptionHistoryService
    {
        private delegate ValueTask<SubscriptionHistory> ReturningSubscriptionHistoryFunction();
        private delegate IQueryable<SubscriptionHistory> ReturningSubscriptionHistoriesFunction();

        private async ValueTask<SubscriptionHistory> TryCatch(
            ReturningSubscriptionHistoryFunction returningSubscriptionHistoryFunction)
        {
            try
            {
                return await returningSubscriptionHistoryFunction();
            }
            catch (NullSubscriptionHistoryException nullSubscriptionHistoryException)
            {
                throw CreateAndLogValidationException(nullSubscriptionHistoryException);
            }
            catch (InvalidSubscriptionHistoryException invalidSubscriptionHistoryException)
            {
                throw CreateAndLogValidationException(invalidSubscriptionHistoryException);
            }
            catch (NotFoundSubscriptionHistoryException notFoundSubscriptionHistoryException)
            {
                throw CreateAndLogValidationException(notFoundSubscriptionHistoryException);
            }
            catch (SqlException sqlException)
            {
                var failedSubscriptionHistoryStorageException =
                    new FailedSubscriptionHistoryStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedSubscriptionHistoryStorageException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedSubscriptionHistoryException =
                    new LockedSubscriptionHistoryException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedSubscriptionHistoryException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedSubscriptionHistoryStorageException =
                    new FailedSubscriptionHistoryStorageException(dbUpdateException);

                throw CreateAndLogDependencyException(failedSubscriptionHistoryStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsSubscriptionHistoryException =
                    new AlreadyExistsSubscriptionHistoryException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsSubscriptionHistoryException);
            }
            catch (Exception exception)
            {
                var failedSubscriptionHistoryServiceException =
                    new FailedSubscriptionHistoryServiceException(exception);

                throw CreateAndLogServiceException(failedSubscriptionHistoryServiceException);
            }
        }

        private IQueryable<SubscriptionHistory> TryCatch(
            ReturningSubscriptionHistoriesFunction returningSubscriptionHistoriesFunction)
        {
            try
            {
                return returningSubscriptionHistoriesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedSubscriptionHistoryStorageException =
                    new FailedSubscriptionHistoryStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedSubscriptionHistoryStorageException);
            }
            catch (Exception exception)
            {
                var failedSubscriptionHistoryServiceException =
                    new FailedSubscriptionHistoryServiceException(exception);

                throw CreateAndLogServiceException(failedSubscriptionHistoryServiceException);
            }
        }

        private SubscriptionHistoryValidationException CreateAndLogValidationException(Xeption exception)
        {
            var SubscriptionHistoryValidationException =
                new SubscriptionHistoryValidationException(exception);

            this.loggingBroker.LogError(SubscriptionHistoryValidationException);

            return SubscriptionHistoryValidationException;
        }

        private SubscriptionHistoryDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var SubscriptionHistoryDependencyException = new SubscriptionHistoryDependencyException(exception);
            this.loggingBroker.LogCritical(SubscriptionHistoryDependencyException);

            return SubscriptionHistoryDependencyException;
        }

        private SubscriptionHistoryDependencyValidationException CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var SubscriptionHistoryDependencyValidationException =
                new SubscriptionHistoryDependencyValidationException(exception);

            this.loggingBroker.LogError(SubscriptionHistoryDependencyValidationException);

            return SubscriptionHistoryDependencyValidationException;
        }

        private SubscriptionHistoryDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var subscriptionHistoryDependencyException =
                new SubscriptionHistoryDependencyException(exception);

            this.loggingBroker.LogError(subscriptionHistoryDependencyException);

            throw subscriptionHistoryDependencyException;
        }

        private SubscriptionHistoryServiceException CreateAndLogServiceException(Xeption exception)
        {
            var SubscriptionHistoryServiceException =
                new SubscriptionHistoryServiceException(exception);

            this.loggingBroker.LogError(SubscriptionHistoryServiceException);

            return SubscriptionHistoryServiceException;
        }
    }
}
