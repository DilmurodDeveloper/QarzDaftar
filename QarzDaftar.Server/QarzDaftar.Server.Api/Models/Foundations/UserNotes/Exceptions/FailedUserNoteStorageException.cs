using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions
{
    public class FailedUserNoteStorageException : Xeption
    {
        public FailedUserNoteStorageException(Exception innerException)
            : base(message: "Failed usernote storage error occurred, contact support.",
                  innerException)
        { }
    }
}
