using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions
{
    public class LockedUserNoteException : Xeption
    {
        public LockedUserNoteException(Exception innerException)
            : base(message: "UserNote is locked, try again later.",
                  innerException)
        { }
    }
}
