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
        }
    }
}
