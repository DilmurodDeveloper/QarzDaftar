using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.UserNotes
{
    public partial class UserNoteService
    {
        private static void ValidateUserNoteNotNull(UserNote UserNote)
        {
            if (UserNote is null)
            {
                throw new NullUserNoteException();
            }
        }
    }
}
