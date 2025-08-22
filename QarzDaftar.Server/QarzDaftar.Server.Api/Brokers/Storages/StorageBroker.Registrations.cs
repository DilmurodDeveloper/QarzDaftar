using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.Registrations;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Registration> Registrations { get; set; }

        public async ValueTask<Registration> InsertRegistrationAsync(Registration registration) =>
            await InsertAsync(registration);

        public IQueryable<Registration> SelectAllRegistrations() =>
            SelectAll<Registration>();
    }
}
