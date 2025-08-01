﻿using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions;
using Xeptions;

namespace QarzDaftar.Server.Api.Services.Foundations.Customers
{
    public partial class CustomerService
    {
        private delegate ValueTask<Customer> ReturningCustomerFunction();

        private async ValueTask<Customer> TryCatch(ReturningCustomerFunction returningCustomerFunction)
        {
            try
            {
                return await returningCustomerFunction();
            }
            catch (NullCustomerException nullCustomerException)
            {
                throw CreateAndLogValidationException(nullCustomerException);
            }
            catch (InvalidCustomerException invalidCustomerException)
            {
                throw CreateAndLogValidationException(invalidCustomerException);
            }
            catch (SqlException sqlException)
            {
                var failedCustomerStorageException =
                    new FailedCustomerStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedCustomerStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsCustomerException =
                    new AlreadyExistsCustomerException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsCustomerException);
            }
            catch (Exception exception)
            {
                var failedCustomerServiceException =
                    new FailedCustomerServiceException(exception);

                throw CreateAndLogServiceException(failedCustomerServiceException);
            }
        }

        private CustomerValidationException CreateAndLogValidationException(Xeption exception)
        {
            var CustomerValidationException = new CustomerValidationException(exception);
            this.loggingBroker.LogError(CustomerValidationException);

            return CustomerValidationException;
        }

        private CustomerDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var CustomerDependencyException = new CustomerDependencyException(exception);
            this.loggingBroker.LogCritical(CustomerDependencyException);

            return CustomerDependencyException;
        }

        private CustomerDependencyValidationException CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var CustomerDependencyValidationException =
                new CustomerDependencyValidationException(exception);

            this.loggingBroker.LogError(CustomerDependencyValidationException);

            return CustomerDependencyValidationException;
        }

        private CustomerServiceException CreateAndLogServiceException(Xeption exception)
        {
            var CustomerServiceException = new CustomerServiceException(exception);
            this.loggingBroker.LogError(CustomerServiceException);

            return CustomerServiceException;
        }
    }
}
