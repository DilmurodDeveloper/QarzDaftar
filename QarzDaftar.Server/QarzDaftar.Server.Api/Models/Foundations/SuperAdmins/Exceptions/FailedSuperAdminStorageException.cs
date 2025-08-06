using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SuperAdmins.Exceptions
{
    public class FailedSuperAdminStorageException : Xeption
    {
        public FailedSuperAdminStorageException(Exception innerException)
            : base(message: "Failed super admin storage error occurred, contact support.",
                  innerException)
        { }
    }
}
