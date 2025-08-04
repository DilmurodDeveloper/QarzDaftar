using QarzDaftar.Server.Api.Models.Foundations.UserNotes;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<UserNote> InsertUserNoteAsync(UserNote userNote);
    }
}
