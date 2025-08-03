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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = GetSqlError();

            var failedCustomerStorageException =
                new FailedCustomerStorageException(sqlException);

            var expectedCustomerDependencyException =
                new CustomerDependencyException(failedCustomerStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomerByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when 
            ValueTask<Customer> retrieveCustomerById =
                this.customerService.RetrieveCustomerByIdAsync(someId);

            CustomerDependencyException actualCustomerDependencyException =
                await Assert.ThrowsAsync<CustomerDependencyException>(
                    retrieveCustomerById.AsTask);

            // then
            actualCustomerDependencyException.Should()
                .BeEquivalentTo(expectedCustomerDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(someId), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCustomerDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdAsyncIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serverException = new Exception();

            var failedCustomerServiceException =
                new FailedCustomerServiceException(serverException);

            var expectedCustomerServiceException =
                new CustomerServiceException(failedCustomerServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomerByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serverException);

            // when
            ValueTask<Customer> retrieveCustomerById =
                this.customerService.RetrieveCustomerByIdAsync(someId);

            CustomerServiceException actualCustomerServiceException =
                await Assert.ThrowsAsync<CustomerServiceException>(
                    retrieveCustomerById.AsTask);

            // then
            actualCustomerServiceException.Should()
                .BeEquivalentTo(expectedCustomerServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(someId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
