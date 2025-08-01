using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Users.Exceptions
{
    public class FailedUserServiceException : Xeption
    {
        public FailedUserServiceException(Exception innerException)
            : base(message: "Failed user service error occurred, please contact support.",
                  innerException)
        { }
    }
}
