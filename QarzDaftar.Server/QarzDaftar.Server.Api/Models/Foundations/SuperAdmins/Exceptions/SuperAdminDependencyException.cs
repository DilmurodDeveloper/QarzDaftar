using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SuperAdmins.Exceptions
{
    public class SuperAdminDependencyException : Xeption
    {
        public SuperAdminDependencyException(Exception innerException)
            : base(message: "Super Admin dependency error occurred, contact support.",
                  innerException)
        { }
    }
}
