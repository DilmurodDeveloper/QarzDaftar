using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.Users;
using QarzDaftar.Server.Api.Models.Foundations.Users.Exceptions;
using Xeptions;

namespace QarzDaftar.Server.Api.Services.Foundations.Users
{
    public partial class UserService
    {
        private delegate ValueTask<User> ReturningUserFunction();
        private delegate IQueryable<User> ReturningUsersFunction();

        private async ValueTask<User> TryCatch(ReturningUserFunction returningUserFunction)
        {
            try
            {
                return await returningUserFunction();
            }
            catch (NullUserException nullUserException)
            {
                throw CreateAndLogValidationException(nullUserException);
            }
            catch (NotFoundUserException notFoundUserException)
            {
                throw CreateAndLogValidationException(notFoundUserException);
            }
            catch (InvalidUserException invalidUserException)
            {
                throw CreateAndLogValidationException(invalidUserException);
            }
            catch (SqlException sqlException)
            {
                var failedUserStorageException =
                    new FailedUserStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedUserStorageException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedUserException =
                    new LockedUserException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedUserException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedUserStorageException =
                    new FailedUserStorageException(dbUpdateException);

                throw CreateAndLogDependencyException(failedUserStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsUserException =
                    new AlreadyExistsUserException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsUserException);
            }
            catch (Exception exception)
            {
                var failedUserServiceException =
                    new FailedUserServiceException(exception);

                throw CreateAndLogServiceException(failedUserServiceException);
            }
        }

        private IQueryable<User> TryCatch(ReturningUsersFunction returningUsersFunction)
        {
            try
            {
                return returningUsersFunction();
            }
            catch (SqlException sqlException)
            {
                var failedUserStorageException =
                    new FailedUserStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedUserStorageException);
            }
            catch (Exception exception)
            {
                var failedUserServiceException =
                    new FailedUserServiceException(exception);

                throw CreateAndLogServiceException(failedUserServiceException);
            }
        }

        private UserValidationException CreateAndLogValidationException(Xeption exception)
        {
            var UserValidationException = new UserValidationException(exception);
            this.loggingBroker.LogError(UserValidationException);

            return UserValidationException;
        }

        private UserDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var UserDependencyException = new UserDependencyException(exception);
            this.loggingBroker.LogCritical(UserDependencyException);

            return UserDependencyException;
        }

        private UserDependencyValidationException CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var UserDependencyValidationException =
                new UserDependencyValidationException(exception);

            this.loggingBroker.LogError(UserDependencyValidationException);

            return UserDependencyValidationException;
        }

        private UserDependencyException CreateAndLogDependencyException(Xeption exception)
        {
            var userDependencyException = new UserDependencyException(exception);
            this.loggingBroker.LogError(userDependencyException);

            throw userDependencyException;
        }

        private UserServiceException CreateAndLogServiceException(Xeption exception)
        {
            var UserServiceException = new UserServiceException(exception);
            this.loggingBroker.LogError(UserServiceException);

            return UserServiceException;
        }
    }
}
