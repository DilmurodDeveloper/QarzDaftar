using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SuperAdmins.Exceptions
{
    public class InvalidSuperAdminException : Xeption
    {
        public InvalidSuperAdminException()
            : base(message: "Super Admin is invalid.")
        { }
    }
}
