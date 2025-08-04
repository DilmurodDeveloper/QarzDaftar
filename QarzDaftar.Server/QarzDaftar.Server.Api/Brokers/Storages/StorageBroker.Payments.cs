using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.Payments;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Payment> Payments { get; set; }

        public async ValueTask<Payment> InsertPaymentAsync(Payment payment) =>
            await InsertAsync(payment);
    }
}
