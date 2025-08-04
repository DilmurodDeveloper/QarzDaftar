using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions
{
    public class UserNoteDependencyValidationException : Xeption
    {
        public UserNoteDependencyValidationException(Xeption innerException)
            : base(message: "UserNote dependency validation error occurred, fix the errors and try again.",
                 innerException)
        { }
    }
}
