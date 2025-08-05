using QarzDaftar.Server.Api.Models.Foundations.UserNotes;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<UserNote> InsertUserNoteAsync(UserNote userNote);
        IQueryable<UserNote> SelectAllUserNotes();
        ValueTask<UserNote> SelectUserNoteByIdAsync(Guid userNoteId);
        ValueTask<UserNote> UpdateUserNoteAsync(UserNote userNote);
        ValueTask<UserNote> DeleteUserNoteAsync(UserNote userNote);
    }
}
