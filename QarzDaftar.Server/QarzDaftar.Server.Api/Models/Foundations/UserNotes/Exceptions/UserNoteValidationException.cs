using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions
{
    public class UserNoteValidationException : Xeption
    {
        public UserNoteValidationException(Xeption innerException)
            : base(message: "UserNote validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
