using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

        public ValueTask<Customer> RetrieveCustomerByIdAsync(Guid customerId) =>
        TryCatch(async () =>
        {
            ValidateCustomerId(customerId);

            Customer maybeCustomer = await this.storageBroker.SelectCustomerByIdAsync(customerId);

            ValidateStorageCustomer(maybeCustomer, customerId);

            return maybeCustomer;
        });

        public async ValueTask<Customer> ModifyCustomerAsync(Customer customer)
        {
            try
            {
                ValidateCustomerOnModify(customer);

                Customer maybeCustomer =
                    await this.storageBroker.SelectCustomerByIdAsync(customer.Id);

                ValidateAgainstStorageCustomerOnModify(customer, maybeCustomer);

                return await this.storageBroker.UpdateCustomerAsync(customer);
            }
            catch (NullCustomerException nullCustomerException)
            {
                var customerValidationException =
                    new CustomerValidationException(nullCustomerException);

                this.loggingBroker.LogError(customerValidationException);

                throw customerValidationException;
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
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedCustomerException =
                    new LockedCustomerException(dbUpdateConcurrencyException);

                var customerDependencyValidationException =
                    new CustomerDependencyValidationException(lockedCustomerException);

                this.loggingBroker.LogError(customerDependencyValidationException);

                throw customerDependencyValidationException;
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedCustomerStorageException =
                    new FailedCustomerStorageException(dbUpdateException);

                var customerDependencyException =
                    new CustomerDependencyException(failedCustomerStorageException);

                this.loggingBroker.LogError(customerDependencyException);

                throw customerDependencyException;
            }
            catch (Exception exception)
            {
                var failedCustomerServiceException =
                    new FailedCustomerServiceException(exception);

                var customerServiceException =
                    new CustomerServiceException(failedCustomerServiceException);

                this.loggingBroker.LogError(customerServiceException);

                throw customerServiceException;
            }
        }
    }
}
