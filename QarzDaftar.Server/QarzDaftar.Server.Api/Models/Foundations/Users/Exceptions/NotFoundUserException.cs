using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Users.Exceptions
{
    public class NotFoundUserException : Xeption
    {
        public NotFoundUserException(Guid userId)
            : base(message: $"User is not found with id: {userId}")
        { }
    }
}
