using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;

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

        public ValueTask<UserNote> RetrieveUserNoteByIdAsync(Guid userNoteId) =>
        TryCatch(async () =>
        {
            ValidateUserNoteId(userNoteId);

            UserNote maybeUserNote =
                await this.storageBroker.SelectUserNoteByIdAsync(userNoteId);

            ValidateStorageUserNote(maybeUserNote, userNoteId);

            return maybeUserNote;
        });

        public ValueTask<UserNote> ModifyUserNoteAsync(UserNote userNote) =>
        TryCatch(async () =>
        {
            ValidateUserNoteOnModify(userNote);

            UserNote maybeUserNote =
                await this.storageBroker.SelectUserNoteByIdAsync(userNote.Id);

            ValidateAgainstStorageUserNoteOnModify(userNote, maybeUserNote);

            return await this.storageBroker.UpdateUserNoteAsync(userNote);
        });

        public async ValueTask<UserNote> RemoveUserNoteByIdAsync(Guid userNoteId)
        {
            UserNote maybeUserNote =
                await this.storageBroker.SelectUserNoteByIdAsync(userNoteId);

            return await this.storageBroker.DeleteUserNoteAsync(maybeUserNote);
        }
    }
}
