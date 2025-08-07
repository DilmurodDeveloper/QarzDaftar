using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Services.Foundations.UserNotes;

namespace QarzDaftar.Server.Api.Services.Processings.UserNotes
{
    public class UserNoteProcessingService : IUserNoteProcessingService
    {
        private readonly IUserNoteService userNoteService;

        public UserNoteProcessingService(IUserNoteService userNoteService) =>
            this.userNoteService = userNoteService;

        public async ValueTask<UserNote> AddUserNoteAsync(UserNote userNote, Guid userId)
        {
            userNote.UserId = userId;
            userNote.CreatedAt = DateTimeOffset.UtcNow;
            userNote.Status = ReminderStatus.Pending;

            return await userNoteService.AddUserNoteAsync(userNote);
        }

        public IQueryable<UserNote> RetrieveAllUserNotesByUserId(Guid userId)
        {
            IQueryable<UserNote> allNotes = userNoteService.RetrieveAllUserNotes();

            return allNotes.Where(note => note.UserId == userId);
        }

        public async ValueTask<UserNote> RetrieveUserNoteByIdAsync(Guid userNoteId) =>
            await userNoteService.RetrieveUserNoteByIdAsync(userNoteId);

        public async ValueTask<UserNote> ModifyUserNoteAsync(UserNote userNote, Guid userId)
        {
            UserNote existingNote = await userNoteService.RetrieveUserNoteByIdAsync(userNote.Id);

            if (existingNote.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to modify this note.");
            }

            return await userNoteService.ModifyUserNoteAsync(userNote);
        }

        public async ValueTask<UserNote> RemoveUserNoteByIdAsync(Guid userNoteId, Guid userId)
        {
            UserNote existingNote = await userNoteService.RetrieveUserNoteByIdAsync(userNoteId);

            if (existingNote.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this note.");
            }

            return await userNoteService.RemoveUserNoteByIdAsync(userNoteId);
        }
    }
}
