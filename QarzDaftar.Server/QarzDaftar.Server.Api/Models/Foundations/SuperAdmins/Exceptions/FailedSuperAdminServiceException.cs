using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SuperAdmins.Exceptions
{
    public class FailedSuperAdminServiceException : Xeption
    {
        public FailedSuperAdminServiceException(Exception innerException)
            : base(message: "Failed super admin service error occurred, please contact support.",
                  innerException)
        { }
    }
}
