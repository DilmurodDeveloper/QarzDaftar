using Microsoft.Data.SqlClient;
using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions;

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

        public async ValueTask<Customer> RetrieveCustomerByIdAsync(Guid customerId)
        {
            try
            {
                ValidateCustomerId(customerId);

                Customer maybeCustomer = await this.storageBroker.SelectCustomerByIdAsync(customerId);

                ValidateStorageCustomer(maybeCustomer, customerId);

                return maybeCustomer;
            }
            catch (InvalidCustomerException invalidCustomerException)
            {
                var customerValidationException =
                    new CustomerValidationException(invalidCustomerException);

                this.loggingBroker.LogError(customerValidationException);

                throw customerValidationException;
            }
            catch (NotFoundCustomerException notFoundCustomerException)
            {
                var customerValidationException =
                    new CustomerValidationException(notFoundCustomerException);

                this.loggingBroker.LogError(customerValidationException);

                throw customerValidationException;
            }
            catch (SqlException sqlException)
            {
                var failedCustomerStorageException =
                    new FailedCustomerStorageException(sqlException);

                var customerDependencyException =
                    new CustomerDependencyException(failedCustomerStorageException);

                this.loggingBroker.LogCritical(customerDependencyException);

                throw customerDependencyException;
            }
        }
    }
}
