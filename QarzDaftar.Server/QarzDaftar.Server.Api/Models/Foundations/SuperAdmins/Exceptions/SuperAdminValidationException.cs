using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SuperAdmins.Exceptions
{
    public class SuperAdminValidationException : Xeption
    {
        public SuperAdminValidationException(Xeption innerException)
            : base(message: "Super Admin validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
