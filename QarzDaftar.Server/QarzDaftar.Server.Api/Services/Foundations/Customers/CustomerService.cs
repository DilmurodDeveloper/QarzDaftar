using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.Customers;

namespace QarzDaftar.Server.Api.Services.Foundations.Customers
{
    public partial class CustomerService : ICustomerService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public CustomerService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<Customer> AddCustomerAsync(Customer customer) =>
        TryCatch(async () =>
        {
            ValidateCustomerOnAdd(customer);

            return await this.storageBroker.InsertCustomerAsync(customer);
        });

        public IQueryable<Customer> RetrieveAllCustomers() =>
            TryCatch(() => this.storageBroker.SelectAllCustomers());

        public ValueTask<Customer> RetrieveCustomerByIdAsync(Guid customerId) =>
        TryCatch(async () =>
        {
            ValidateCustomerId(customerId);

            Customer maybeCustomer = await this.storageBroker.SelectCustomerByIdAsync(customerId);

            ValidateStorageCustomer(maybeCustomer, customerId);

            return maybeCustomer;
        });

        public ValueTask<Customer> ModifyCustomerAsync(Customer customer) =>
            throw new NotImplementedException();
    }
}
