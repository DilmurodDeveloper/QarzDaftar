using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions
{
    public class FailedUserNoteServiceException : Xeption
    {
        public FailedUserNoteServiceException(Exception innerException)
            : base(message: "Failed usernote service error occurred, please contact support.",
                  innerException)
        { }
    }
}
