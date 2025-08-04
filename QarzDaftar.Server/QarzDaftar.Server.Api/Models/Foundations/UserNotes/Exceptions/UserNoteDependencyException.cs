using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions
{
    public class UserNoteDependencyException : Xeption
    {
        public UserNoteDependencyException(Exception innerException)
            : base(message: "UserNote dependency error occurred, contact support.",
                  innerException)
        { }
    }
}
