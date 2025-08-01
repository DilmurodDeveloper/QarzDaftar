using QarzDaftar.Server.Api.Models.Foundations.Users;
using QarzDaftar.Server.Api.Models.Foundations.Users.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundatios.Users
{
    public partial class UserService
    {
        private static void ValidateUserNotNull(User user)
        {
            if (user is null)
            {
                throw new NullUserException();
            }
        }
    }
}
