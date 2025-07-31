using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<UserPaymentLog> UserPaymentLogs { get; set; }
    }
}
