using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;

namespace QarzDaftar.Server.Api.Services.Foundations.SuperAdmins
{
    public interface ISuperAdminService
    {
        ValueTask<SuperAdmin> RetrieveSuperAdminByUsernameAsync(string username);
    }
}
