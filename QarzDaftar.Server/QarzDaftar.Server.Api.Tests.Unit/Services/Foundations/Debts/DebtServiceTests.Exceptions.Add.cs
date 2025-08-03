using Microsoft.Data.SqlClient;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursLogItAsync()
        {
            // given
            Debt someDebt = CreateRandomDebt();
            SqlException sqlException = GetSqlError();

            var failedDebtStorageException =
                new FailedDebtStorageException(sqlException);

            var expectedDebtDependencyException =
                new DebtDependencyException(failedDebtStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertDebtAsync(someDebt))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Debt> addDebtTask =
                this.debtService.AddDebtAsync(someDebt);

            // then
            await Assert.ThrowsAsync<DebtDependencyException>(() =>
                addDebtTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDebtAsync(someDebt), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedDebtDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
