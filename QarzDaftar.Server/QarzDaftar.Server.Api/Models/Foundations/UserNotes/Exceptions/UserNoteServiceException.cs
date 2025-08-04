using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions
{
    public class UserNoteServiceException : Xeption
    {
        public UserNoteServiceException(Exception innerException)
            : base(message: "UserNote service error occurred, contact support.",
                  innerException)
        { }
    }
}
