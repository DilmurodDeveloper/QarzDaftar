using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions
{
    public class InvalidUserNoteException : Xeption
    {
        public InvalidUserNoteException()
            : base(message: "UserNote is invalid.")
        { }
    }
}
