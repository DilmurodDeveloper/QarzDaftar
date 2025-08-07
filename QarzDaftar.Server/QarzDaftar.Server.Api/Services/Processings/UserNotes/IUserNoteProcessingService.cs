using QarzDaftar.Server.Api.Models.Foundations.UserNotes;

namespace QarzDaftar.Server.Api.Services.Processings.UserNotes
{
    public interface IUserNoteProcessingService
    {
        ValueTask<UserNote> AddUserNoteAsync(UserNote userNote, Guid userId);
        IQueryable<UserNote> RetrieveAllUserNotesByUserId(Guid userId);
        ValueTask<UserNote> RetrieveUserNoteByIdAsync(Guid userNoteId);
        ValueTask<UserNote> ModifyUserNoteAsync(UserNote userNote, Guid userId);
        ValueTask<UserNote> RemoveUserNoteByIdAsync(Guid userNoteId, Guid userId);
    }
}
