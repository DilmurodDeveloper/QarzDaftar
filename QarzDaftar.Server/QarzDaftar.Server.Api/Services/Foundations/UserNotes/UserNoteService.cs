using Microsoft.Data.SqlClient;
using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.UserNotes
{
    public partial class UserNoteService : IUserNoteService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public UserNoteService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<UserNote> AddUserNoteAsync(UserNote userNote) =>
        TryCatch(async () =>
        {
            ValidateUserNoteOnAdd(userNote);

            return await this.storageBroker.InsertUserNoteAsync(userNote);
        });

        public IQueryable<UserNote> RetrieveAllUserNotes() =>
            TryCatch(() => this.storageBroker.SelectAllUserNotes());

        public async ValueTask<UserNote> RetrieveUserNoteByIdAsync(Guid userNoteId)
        {
            try
            {
                ValidateUserNoteId(userNoteId);

                UserNote maybeUserNote =
                    await this.storageBroker.SelectUserNoteByIdAsync(userNoteId);

                ValidateStorageUserNote(maybeUserNote, userNoteId);

                return maybeUserNote;
            }
            catch (InvalidUserNoteException invalidUserNoteException)
            {
                var userNoteValidationException =
                    new UserNoteValidationException(invalidUserNoteException);

                this.loggingBroker.LogError(userNoteValidationException);

                throw userNoteValidationException;
            }
            catch (NotFoundUserNoteException notFoundUserNoteException)
            {
                var userNoteValidationException =
                    new UserNoteValidationException(notFoundUserNoteException);

                this.loggingBroker.LogError(userNoteValidationException);

                throw userNoteValidationException;
            }
            catch (SqlException sqlException)
            {
                var failedUserNoteStorageException =
                    new FailedUserNoteStorageException(sqlException);

                var userNoteDependencyException =
                    new UserNoteDependencyException(failedUserNoteStorageException);

                this.loggingBroker.LogCritical(userNoteDependencyException);

                throw userNoteDependencyException;
            }
            catch (Exception exception)
            {
                var failedUserNoteServiceException =
                    new FailedUserNoteServiceException(exception);

                var userNoteServiceException =
                    new UserNoteServiceException(failedUserNoteServiceException);

                this.loggingBroker.LogError(userNoteServiceException);

                throw userNoteServiceException;
            }
        }
    }
}
