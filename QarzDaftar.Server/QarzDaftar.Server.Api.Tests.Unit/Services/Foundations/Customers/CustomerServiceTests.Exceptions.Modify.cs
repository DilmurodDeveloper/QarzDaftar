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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Customer randomCustomer = CreateRandomCustomer();
            Customer someCustomer = randomCustomer;
            Guid customerId = someCustomer.Id;
            SqlException sqlException = GetSqlError();

            var failedCustomerStorageException =
                new FailedCustomerStorageException(sqlException);

            var expectedCustomerDependencyException =
                new CustomerDependencyException(failedCustomerStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomerByIdAsync(customerId)).Throws(sqlException);

            // when
            ValueTask<Customer> modifyCustomerTask =
                this.customerService.ModifyCustomerAsync(someCustomer);

            CustomerDependencyException actualCustomerDependencyException =
                 await Assert.ThrowsAsync<CustomerDependencyException>(
                    modifyCustomerTask.AsTask);

            // then
            actualCustomerDependencyException.Should()
                .BeEquivalentTo(expectedCustomerDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCustomerDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(customerId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomerAsync(someCustomer), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            Customer randomCustomer = CreateRandomCustomer();
            Customer someCustomer = randomCustomer;
            Guid customerId = someCustomer.Id;
            var databaseUpdateException = new DbUpdateException();

            var failedCustomerStorageException =
                new FailedCustomerStorageException(databaseUpdateException);

            var expectedCustomerDependencyException =
                new CustomerDependencyException(failedCustomerStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomerByIdAsync(customerId)).Throws(databaseUpdateException);

            // when
            ValueTask<Customer> modifyCustomerTask =
                this.customerService.ModifyCustomerAsync(someCustomer);

            CustomerDependencyException actualCustomerDependencyException =
                 await Assert.ThrowsAsync<CustomerDependencyException>(
                    modifyCustomerTask.AsTask);

            // then
            actualCustomerDependencyException.Should()
                .BeEquivalentTo(expectedCustomerDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(customerId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomerAsync(someCustomer), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Customer randomCustomer = CreateRandomCustomer();
            Customer someCustomer = randomCustomer;
            Guid customerId = someCustomer.Id;
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedCustomerException =
                new LockedCustomerException(dbUpdateConcurrencyException);

            var expectedCustomerDependencyValidationException =
                new CustomerDependencyValidationException(lockedCustomerException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomerByIdAsync(customerId))
                    .Throws(dbUpdateConcurrencyException);

            // when
            ValueTask<Customer> modifyCustomerTask =
                this.customerService.ModifyCustomerAsync(someCustomer);

            CustomerDependencyValidationException actualCustomerDependencyValidationException =
                 await Assert.ThrowsAsync<CustomerDependencyValidationException>(
                    modifyCustomerTask.AsTask);

            // then
            actualCustomerDependencyValidationException.Should()
                .BeEquivalentTo(expectedCustomerDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(customerId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomerAsync(someCustomer), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Customer randomCustomer = CreateRandomCustomer();
            Customer someCustomer = randomCustomer;
            Guid customerId = someCustomer.Id;
            var serviceException = new Exception();

            var failedCustomerServiceException =
                new FailedCustomerServiceException(serviceException);

            var expectedCustomerServiceException =
                new CustomerServiceException(failedCustomerServiceException);

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectCustomerByIdAsync(customerId))
                .Throws(serviceException);

            // when
            ValueTask<Customer> modifyCustomerTask =
                this.customerService.ModifyCustomerAsync(someCustomer);

            CustomerServiceException actualCustomerServiceException =
                await Assert.ThrowsAsync<CustomerServiceException>(
                    modifyCustomerTask.AsTask);

            // then
            actualCustomerServiceException.Should()
                .BeEquivalentTo(expectedCustomerServiceException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerServiceException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(customerId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomerAsync(someCustomer), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
