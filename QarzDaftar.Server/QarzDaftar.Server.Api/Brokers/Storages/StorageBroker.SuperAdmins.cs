using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<SuperAdmin> SuperAdmins { get; set; }
    }
}
