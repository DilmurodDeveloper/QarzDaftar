using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<UserNote> UserNotes { get; set; }

        public async ValueTask<UserNote> InsertUserNoteAsync(UserNote userNote) =>
            await InsertAsync(userNote);

        public IQueryable<UserNote> SelectAllUserNotes()
        {
            var userNotes = SelectAll<UserNote>()
                .Include(c => c.User);

            return userNotes;
        }

        public async ValueTask<UserNote> SelectUserNoteByIdAsync(Guid userNoteId)
        {
            var userNoteWithDetails = UserNotes
                .Include(c => c.User)
                .FirstOrDefault(c => c.Id == userNoteId);

            return await ValueTask.FromResult(userNoteWithDetails);
        }
    }
}
