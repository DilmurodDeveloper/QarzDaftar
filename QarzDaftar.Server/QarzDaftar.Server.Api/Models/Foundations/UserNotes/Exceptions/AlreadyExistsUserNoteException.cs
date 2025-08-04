using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions
{
    public class AlreadyExistsUserNoteException : Xeption
    {
        public AlreadyExistsUserNoteException(Exception innerException)
            : base(message: "UserNote already exists.", innerException)
        { }
    }
}
