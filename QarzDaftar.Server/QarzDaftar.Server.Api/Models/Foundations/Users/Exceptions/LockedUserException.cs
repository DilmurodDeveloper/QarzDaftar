using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Users.Exceptions
{
    public class LockedUserException : Xeption
    {
        public LockedUserException(Exception innerException)
            : base(message: "User is locked, try again later.",
                  innerException)
        { }
    }
}
