using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions
{
    public class NotFoundUserNoteException : Xeption
    {
        public NotFoundUserNoteException(Guid userNoteId)
            : base(message: $"UserNote is not found with id: {userNoteId}")
        { }
    }
}
