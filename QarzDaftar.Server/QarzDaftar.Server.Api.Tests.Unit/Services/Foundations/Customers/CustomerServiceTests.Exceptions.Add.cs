using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Customers
{
    public partial class CustomerServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursLogItAsync()
        {
            // given
            Customer someCustomer = CreateRandomCustomer();
            SqlException sqlException = GetSqlError();

            var failedCustomerStorageException =
                new FailedCustomerStorageException(sqlException);

            var expectedCustomerDependencyException =
                new CustomerDependencyException(failedCustomerStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertCustomerAsync(someCustomer))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Customer> addCustomerTask =
                this.customerService.AddCustomerAsync(someCustomer);

            // then
            await Assert.ThrowsAsync<CustomerDependencyException>(() =>
                addCustomerTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCustomerAsync(someCustomer), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCustomerDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccursAndLogItAsync()
        {
            // given
            string someMessage = GetRandomString();
            Customer someCustomer = CreateRandomCustomer();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsCustomerException =
                new AlreadyExistsCustomerException(duplicateKeyException);

            var expectedCustomerDependencyValidationException =
                new CustomerDependencyValidationException(alreadyExistsCustomerException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertCustomerAsync(someCustomer)).ThrowsAsync(duplicateKeyException);
            // when
            ValueTask<Customer> addCustomerTask =
                this.customerService.AddCustomerAsync(someCustomer);

            var actualCustomerDependencyValidationException = await Assert
                .ThrowsAsync<CustomerDependencyValidationException>(addCustomerTask.AsTask);

            // then
            actualCustomerDependencyValidationException.Should()
                .BeEquivalentTo(expectedCustomerDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCustomerAsync(someCustomer), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
