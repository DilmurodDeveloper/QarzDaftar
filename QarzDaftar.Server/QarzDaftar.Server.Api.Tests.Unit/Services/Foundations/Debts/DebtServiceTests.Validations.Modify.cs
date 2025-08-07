using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionIfDebtIsNullAndLogItAsync()
        {
            // given
            Debt nullDebt = null;
            var nullDebtException = new NullDebtException();

            var expectedDebtValidationException =
                new DebtValidationException(nullDebtException);

            // when
            ValueTask<Debt> modifyDebtTask =
                this.debtService.ModifyDebtAsync(nullDebt);

            DebtValidationException actualDebtValidationException =
                await Assert.ThrowsAsync<DebtValidationException>(() =>
                    modifyDebtTask.AsTask());

            // then
            actualDebtValidationException.Should()
                .BeEquivalentTo(expectedDebtValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDebtAsync(It.IsAny<Debt>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfDebtIsInvalidAndLogItAsync(
            string invalidString)
        {
            //given
            Debt invalidDebt = new Debt
            {
                Description = invalidString
            };

            var invalidDebtException = new InvalidDebtException();

            invalidDebtException.AddData(
                key: nameof(Debt.Id),
                values: "Id is required");

            invalidDebtException.AddData(
                key: nameof(Debt.Amount),
                values: "Amount is required");

            invalidDebtException.AddData(
                key: nameof(Debt.RemainingAmount),
                values: "Amount is required");

            invalidDebtException.AddData(
                key: nameof(Debt.Description),
                values: "Text is required");

            invalidDebtException.AddData(
                key: nameof(Debt.DueDate),
                values: "Date is required");

            invalidDebtException.AddData(
                key: nameof(Debt.CreatedDate),
                values: "Date is required");

            invalidDebtException.AddData(
                key: nameof(Debt.UpdatedDate),
                values: "Date is required");

            invalidDebtException.AddData(
                key: nameof(Debt.CustomerId),
                values: "Id is required");

            var expectedDebtValidationException =
                new DebtValidationException(invalidDebtException);

            // when
            ValueTask<Debt> modifyDebtTask =
                this.debtService.ModifyDebtAsync(invalidDebt);

            DebtValidationException actualDebtValidationException =
                await Assert.ThrowsAsync<DebtValidationException>(
                    modifyDebtTask.AsTask);

            // then
            actualDebtValidationException.Should()
                .BeEquivalentTo(expectedDebtValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDebtAsync(It.IsAny<Debt>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfDebtDoesNotExistAndLogItAsync()
        {
            // given
            Debt randomDebt = CreateRandomDebt();
            Debt nonExistDebt = randomDebt;
            Debt nullDebt = null;

            var notFoundDebtException =
                new NotFoundDebtException(nonExistDebt.Id);

            var expectedDebtValidationException =
                new DebtValidationException(notFoundDebtException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDebtByIdAsync(nonExistDebt.Id))
                    .ReturnsAsync(nullDebt);

            // when
            ValueTask<Debt> modifyDebtTask =
                this.debtService.ModifyDebtAsync(nonExistDebt);

            DebtValidationException actualDebtValidationException =
                await Assert.ThrowsAsync<DebtValidationException>
                    (modifyDebtTask.AsTask);

            // then
            actualDebtValidationException.Should()
                .BeEquivalentTo(expectedDebtValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDebtByIdAsync(nonExistDebt.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDebtAsync(nonExistDebt), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
