using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SuperAdmins.Exceptions
{
    public class NotFoundSuperAdminException : Xeption
    {
        public NotFoundSuperAdminException(string username)
            : base(message: $"Super Admin is not found with id: {username}")
        { }
    }
}
