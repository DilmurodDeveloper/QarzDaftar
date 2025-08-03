using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given 
            Guid invalidDebtId = Guid.Empty;
            var invalidDebtException = new InvalidDebtException();

            invalidDebtException.AddData(
                key: nameof(Debt.Id),
                values: "Id is required");

            var expectedDebtValidationException =
                new DebtValidationException(invalidDebtException);

            // when
            ValueTask<Debt> removeDebtById =
                this.debtService.RemoveDebtByIdAsync(invalidDebtId);

            DebtValidationException actualDebtValidationException =
                await Assert.ThrowsAsync<DebtValidationException>(
                    removeDebtById.AsTask);
            // then
            actualDebtValidationException.Should()
                .BeEquivalentTo(expectedDebtValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDebtByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.storageBrokerMock.Verify(broker =>
            broker.DeleteDebtAsync(It.IsAny<Debt>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveDebtByIdIsNotFoundAndLogItAsync()
        {
            // given
            Guid inputDebtId = Guid.NewGuid();
            Debt noDebt = null;
            var notFoundDebtException = new NotFoundDebtException(inputDebtId);

            var expectedDebtValidationException =
                new DebtValidationException(notFoundDebtException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDebtByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noDebt);

            // when
            ValueTask<Debt> removeDebtById =
                this.debtService.RemoveDebtByIdAsync(inputDebtId);

            var actualDebtValidationException =
                await Assert.ThrowsAsync<DebtValidationException>(
                    removeDebtById.AsTask);

            // then
            actualDebtValidationException.Should()
                .BeEquivalentTo(expectedDebtValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDebtByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteDebtAsync(It.IsAny<Debt>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
