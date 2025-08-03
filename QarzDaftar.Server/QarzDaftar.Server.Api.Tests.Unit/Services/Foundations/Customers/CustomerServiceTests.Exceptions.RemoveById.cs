using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Customers
{
    public partial class CustomerServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someCustomerId = Guid.NewGuid();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedCustomerException =
                new LockedCustomerException(dbUpdateConcurrencyException);

            var expectedCustomerDependencyValidationException =
                new CustomerDependencyValidationException(lockedCustomerException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomerByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<Customer> removeCustomerById =
                this.customerService.RemoveCustomerByIdAsync(someCustomerId);

            var actualCustomerDependencyValidationException =
                await Assert.ThrowsAsync<CustomerDependencyValidationException>(
                    removeCustomerById.AsTask);

            // then
            actualCustomerDependencyValidationException.Should()
                .BeEquivalentTo(expectedCustomerDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCustomerAsync(It.IsAny<Customer>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someCustomerId = Guid.NewGuid();
            SqlException sqlException = GetSqlError();

            var failedCustomerStorageException =
                new FailedCustomerStorageException(sqlException);

            var expectedCustomerDependencyException =
                new CustomerDependencyException(failedCustomerStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomerByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);
            // when
            ValueTask<Customer> deleteCustomerTask =
                this.customerService.RemoveCustomerByIdAsync(someCustomerId);

            CustomerDependencyException actualCustomerDependencyException =
                await Assert.ThrowsAsync<CustomerDependencyException>(
                    deleteCustomerTask.AsTask);

            // then
            actualCustomerDependencyException.Should()
                .BeEquivalentTo(expectedCustomerDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCustomerDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someCustomerId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedCustomerServiceException =
                new FailedCustomerServiceException(serviceException);

            var expectedCustomerServiceException =
                new CustomerServiceException(failedCustomerServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomerByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Customer> removeCustomerByIdTask =
                this.customerService.RemoveCustomerByIdAsync(someCustomerId);

            CustomerServiceException actualCustomerServiceException =
                await Assert.ThrowsAsync<CustomerServiceException>(
                    removeCustomerByIdTask.AsTask);

            // then
            actualCustomerServiceException.Should()
                .BeEquivalentTo(expectedCustomerServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
