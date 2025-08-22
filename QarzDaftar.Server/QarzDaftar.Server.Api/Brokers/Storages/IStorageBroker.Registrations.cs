using QarzDaftar.Server.Api.Models.Foundations.Registrations;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Registration> InsertRegistrationAsync(Registration registration);
        IQueryable<Registration> SelectAllRegistrations();
    }
}
