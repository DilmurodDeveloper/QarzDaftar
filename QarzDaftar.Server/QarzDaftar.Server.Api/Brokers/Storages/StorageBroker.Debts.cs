using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.Debts;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Debt> Debts { get; set; }
    }
}
