using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
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
            ValueTask<Debt> retrieveDebtById =
                this.debtService.RetrieveDebtByIdAsync(invalidDebtId);

            DebtValidationException actualDebtValidationException =
                await Assert.ThrowsAsync<DebtValidationException>(
                    retrieveDebtById.AsTask);

            // then
            actualDebtValidationException.Should()
                .BeEquivalentTo(expectedDebtValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDebtByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfDebtNotFoundAndLogItAsync()
        {
            // given
            Guid someDebtId = Guid.NewGuid();
            Debt noDebt = null;

            var notFoundDebtException =
                new NotFoundDebtException(someDebtId);

            var expetedDebtValidationException =
                new DebtValidationException(notFoundDebtException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDebtByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noDebt);

            // when
            ValueTask<Debt> retriveByIdDebtTask =
                this.debtService.RetrieveDebtByIdAsync(someDebtId);

            DebtValidationException actualDebtValidationException =
                await Assert.ThrowsAsync<DebtValidationException>(
                    retriveByIdDebtTask.AsTask);

            // then
            actualDebtValidationException.Should()
                .BeEquivalentTo(expetedDebtValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDebtByIdAsync(someDebtId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expetedDebtValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
