using QarzDaftar.Server.Api.Models.Foundations.Registrations;

namespace QarzDaftar.Server.Api.Services.Foundations.Registrations
{
    public interface IRegistrationService
    {
        ValueTask<Registration> AddRegistrationAsync(Registration registration);
        IQueryable<Registration> RetrieveAllRegistrations();
    }
}
