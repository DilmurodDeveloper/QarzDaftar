using EFxceptions;
using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Payments;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker : EFxceptionsContext, IStorageBroker
    {
        private readonly IConfiguration configuration;

        public StorageBroker(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.Database.Migrate();
        }

        public async ValueTask<T> InsertAsync<T>(T @object)
        {
            var broker = new StorageBroker(this.configuration);
            broker.Entry(@object).State = EntityState.Added;
            await broker.SaveChangesAsync();

            return @object;
        }

        public IQueryable<T> SelectAll<T>() where T : class
        {
            var broker = new StorageBroker(configuration);

            return broker.Set<T>();
        }

        public async ValueTask<T> UpdateAsync<T>(T @object)
        {
            var broker = new StorageBroker(configuration);
            broker.Entry(@object).State = EntityState.Modified;
            await broker.SaveChangesAsync();

            return @object;
        }

        public async ValueTask<T> DeleteAsync<T>(T @object)
        {
            var broker = new StorageBroker(configuration);
            broker.Entry(@object).State = EntityState.Deleted;
            await broker.SaveChangesAsync();

            return @object;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString =
                this.configuration.GetConnectionString(name: "DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Debt>()
                .HasOne(d => d.Customer)
                .WithMany(c => c.Debts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Debt>()
                .Property(d => d.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Debt>()
                .Property(d => d.RemainingAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithMany(u => u.Customers)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Payments)
                .WithOne(p => p.Customer)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Customer)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserNote>()
                .HasOne(n => n.User)
                .WithMany(u => u.UserNotes)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserPaymentLog>()
                .HasOne(l => l.User)
                .WithMany(u => u.PaymentLogs)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserPaymentLog>()
                .Property(l => l.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<SubscriptionHistory>()
                .HasOne(s => s.User)
                .WithMany(u => u.SubscriptionHistories)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubscriptionHistory>()
                .Property(s => s.Amount)
                .HasColumnType("decimal(18,2)");
        }

        public override void Dispose() { }
    }
}