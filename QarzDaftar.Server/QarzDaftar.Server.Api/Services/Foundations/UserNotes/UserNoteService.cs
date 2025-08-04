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

        public async ValueTask<UserNote> AddUserNoteAsync(UserNote userNote)
        {
            try
            {
                ValidateUserNoteOnAdd(userNote);

                return await this.storageBroker.InsertUserNoteAsync(userNote);
            }
            catch (NullUserNoteException nullUserNoteException)
            {
                var userNoteValidationException =
                    new UserNoteValidationException(nullUserNoteException);

                this.loggingBroker.LogError(userNoteValidationException);

                throw userNoteValidationException;
            }
            catch (InvalidUserNoteException invalidUserNoteException)
            {
                var userNoteValidationException =
                    new UserNoteValidationException(invalidUserNoteException);

                this.loggingBroker.LogError(userNoteValidationException);

                throw userNoteValidationException;
            }
        }
    }
}
