using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SuperAdmins.Exceptions
{
    public class SuperAdminServiceException : Xeption
    {
        public SuperAdminServiceException(Exception innerException)
            : base(message: "Super Admin service error occurred, contact support.",
                  innerException)
        { }
    }
}
