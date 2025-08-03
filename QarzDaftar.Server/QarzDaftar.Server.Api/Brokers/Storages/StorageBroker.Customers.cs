using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.Customers;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Customer> Customers { get; set; }

        public async ValueTask<Customer> InsertCustomerAsync(Customer customer) =>
            await InsertAsync(customer);

        public IQueryable<Customer> SelectAllCustomers()
        {
            var customers = SelectAll<Customer>()
                .Include(c => c.Debts)
                .Include(c => c.Payments);

            return customers;
        }

        public async ValueTask<Customer> SelectCustomerByIdAsync(Guid customerId)
        {
            var customerWithDetails = Customers
                .Include(c => c.Debts)
                .Include(c => c.Payments)
                .FirstOrDefault(c => c.Id == customerId);

            return await ValueTask.FromResult(customerWithDetails);
        }

        public async ValueTask<Customer> UpdateCustomerAsync(Customer customer) =>
            await UpdateAsync(customer);
    }
}
