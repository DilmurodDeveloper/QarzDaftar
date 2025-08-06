using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Users.Exceptions
{
    public class NotFoundUserException : Xeption
    {
        public NotFoundUserException(Guid userId)
            : base(message: $"User is not found with id: {userId}")
        { }

        public NotFoundUserException(string username)
            : base(message: $"Couldn't find user with username: {username}.")
        {
            this.AddData(
                key: nameof(User.Username),
                values: $"User with username '{username}' not found.");
        }
    }
}
