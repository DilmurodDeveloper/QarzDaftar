using QarzDaftar.Server.Api.Models.Foundations.UserNotes;

namespace QarzDaftar.Server.Api.Services.Foundations.UserNotes
{
    public interface IUserNoteService
    {
        ValueTask<UserNote> AddUserNoteAsync(UserNote userNote);
        IQueryable<UserNote> RetrieveAllUserNotes();
        ValueTask<UserNote> RetrieveUserNoteByIdAsync(Guid userNoteId);
        ValueTask<UserNote> ModifyUserNoteAsync(UserNote userNote);
        ValueTask<UserNote> RemoveUserNoteByIdAsync(Guid userNoteId);
    }
}
