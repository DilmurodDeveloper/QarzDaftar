using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Users.Exceptions
{
    public class UserDependencyException : Xeption
    {
        public UserDependencyException(Exception innerException)
            : base(message: "User dependency error occurred, contact support.",
                  innerException)
        { }
    }
}
