using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions
{
    public class NullUserNoteException : Xeption
    {
        public NullUserNoteException()
            : base(message: "UserNote is null.")
        { }
    }
}
