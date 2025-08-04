using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;
using Xeptions;

namespace QarzDaftar.Server.Api.Services.Foundations.UserNotes
{
    public partial class UserNoteService
    {
        private delegate ValueTask<UserNote> ReturningUserNoteFunction();
        private delegate IQueryable<UserNote> ReturningUserNotesFunction();

        private async ValueTask<UserNote> TryCatch(ReturningUserNoteFunction returningUserNoteFunction)
        {
            try
            {
                return await returningUserNoteFunction();
            }
            catch (NullUserNoteException nullUserNoteException)
            {
                throw CreateAndLogValidationException(nullUserNoteException);
            }
            catch (InvalidUserNoteException invalidUserNoteException)
            {
                throw CreateAndLogValidationException(invalidUserNoteException);
            }
            catch (SqlException sqlException)
            {
                var failedUserNoteStorageException =
                    new FailedUserNoteStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedUserNoteStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsUserNoteException =
                    new AlreadyExistsUserNoteException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsUserNoteException);
            }
            catch (Exception exception)
            {
                var failedUserNoteServiceException =
                    new FailedUserNoteServiceException(exception);

                throw CreateAndLogServiceException(failedUserNoteServiceException);
            }
        }

        private IQueryable<UserNote> TryCatch(ReturningUserNotesFunction returningUserNotesFunction)
        {
            try
            {
                return returningUserNotesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedUserNoteStorageException =
                    new FailedUserNoteStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedUserNoteStorageException);
            }
            catch (Exception exception)
            {
                var failedUserNoteServiceException =
                    new FailedUserNoteServiceException(exception);

                throw CreateAndLogServiceException(failedUserNoteServiceException);
            }
        }

        private UserNoteValidationException CreateAndLogValidationException(Xeption exception)
        {
            var UserNoteValidationException = new UserNoteValidationException(exception);
            this.loggingBroker.LogError(UserNoteValidationException);

            return UserNoteValidationException;
        }

        private UserNoteDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var UserNoteDependencyException = new UserNoteDependencyException(exception);
            this.loggingBroker.LogCritical(UserNoteDependencyException);

            return UserNoteDependencyException;
        }

        private UserNoteDependencyValidationException CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var UserNoteDependencyValidationException =
                new UserNoteDependencyValidationException(exception);

            this.loggingBroker.LogError(UserNoteDependencyValidationException);

            return UserNoteDependencyValidationException;
        }

        private UserNoteServiceException CreateAndLogServiceException(Xeption exception)
        {
            var UserNoteServiceException = new UserNoteServiceException(exception);
            this.loggingBroker.LogError(UserNoteServiceException);

            return UserNoteServiceException;
        }
    }
}
