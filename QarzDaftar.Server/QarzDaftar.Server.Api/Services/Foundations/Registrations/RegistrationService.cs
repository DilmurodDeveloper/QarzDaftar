using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.Registrations;

namespace QarzDaftar.Server.Api.Services.Foundations.Registrations
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IStorageBroker storageBroker;

        public RegistrationService(IStorageBroker storageBroker) =>
            this.storageBroker = storageBroker;

        public async ValueTask<Registration> AddRegistrationAsync(Registration registration) =>
            await this.storageBroker.InsertRegistrationAsync(registration);

        public IQueryable<Registration> RetrieveAllRegistrations() =>
            this.storageBroker.SelectAllRegistrations();
    }
}
