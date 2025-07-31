using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.Customers;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Customer> Customers { get; set; }
    }
}
