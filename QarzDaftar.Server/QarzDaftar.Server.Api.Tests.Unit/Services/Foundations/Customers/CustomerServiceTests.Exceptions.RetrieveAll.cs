using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Customers
{
    public partial class CustomerServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given 
            SqlException sqlException = GetSqlError();

            var failedCustomerStorageException =
                new FailedCustomerStorageException(sqlException);

            var expectedCustomerDependencyException =
                new CustomerDependencyException(failedCustomerStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCustomers()).Throws(sqlException);

            // when
            Action retrieveAllCustomersAction = () =>
                this.customerService.RetrieveAllCustomers();

            CustomerDependencyException actualCustomerDependencyException =
                Assert.Throws<CustomerDependencyException>(retrieveAllCustomersAction);

            // then
            actualCustomerDependencyException.Should()
                .BeEquivalentTo(expectedCustomerDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCustomers(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedCustomerDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomString();
            var serverException = new Exception(exceptionMessage);

            var failedCustomerServiceException =
                new FailedCustomerServiceException(serverException);

            var expectedCustomerServiceException =
                new CustomerServiceException(failedCustomerServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCustomers()).Throws(serverException);

            // when 
            Action retrieveAllCustomerActions = () =>
                this.customerService.RetrieveAllCustomers();

            CustomerServiceException actualCustomerServiceException =
                Assert.Throws<CustomerServiceException>(retrieveAllCustomerActions);

            // then
            actualCustomerServiceException.Should()
                .BeEquivalentTo(expectedCustomerServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCustomers(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
