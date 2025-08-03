using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public async Task ShouldThrowSqlExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = GetSqlError();

            var failedDebtStorageException =
                new FailedDebtStorageException(sqlException);

            var expectedDebtDependencyException =
                new DebtDependencyException(failedDebtStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDebtByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Debt> retrieveDebtByIdTask =
                this.debtService.RetrieveDebtByIdAsync(someId);

            DebtDependencyException actualDebtDependencyException =
                await Assert.ThrowsAsync<DebtDependencyException>(
                    retrieveDebtByIdTask.AsTask);

            // then
            actualDebtDependencyException.Should()
                .BeEquivalentTo(expectedDebtDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDebtByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedDebtDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedDebtServiceException =
                new FailedDebtServiceException(serviceException);

            var expectedDebtServiceException =
                new DebtServiceException(failedDebtServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDebtByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Debt> retrieveDebtByIdTask =
                this.debtService.RetrieveDebtByIdAsync(someId);

            DebtServiceException actualDebtServiceException =
                await Assert.ThrowsAsync<DebtServiceException>(
                    retrieveDebtByIdTask.AsTask);

            // then
            actualDebtServiceException.Should()
                .BeEquivalentTo(expectedDebtServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDebtByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
