using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given 
            SqlException sqlException = GetSqlError();

            var failedDebtStorageException =
                new FailedDebtStorageException(sqlException);

            var expectedDebtDependencyException =
                new DebtDependencyException(failedDebtStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllDebts()).Throws(sqlException);

            // when
            Action retrieveAllDebtsAction = () =>
                this.debtService.RetrieveAllDebts();

            DebtDependencyException actualDebtDependencyException =
                Assert.Throws<DebtDependencyException>(retrieveAllDebtsAction);

            // then
            actualDebtDependencyException.Should()
                .BeEquivalentTo(expectedDebtDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDebts(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedDebtDependencyException))), Times.Once);

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

            var failedDebtServiceException =
                new FailedDebtServiceException(serverException);

            var expectedDebtServiceException =
                new DebtServiceException(failedDebtServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllDebts()).Throws(serverException);

            // when 
            Action retrieveAllDebtActions = () =>
                this.debtService.RetrieveAllDebts();

            DebtServiceException actualDebtServiceException =
                Assert.Throws<DebtServiceException>(retrieveAllDebtActions);

            // then
            actualDebtServiceException.Should()
                .BeEquivalentTo(expectedDebtServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDebts(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
