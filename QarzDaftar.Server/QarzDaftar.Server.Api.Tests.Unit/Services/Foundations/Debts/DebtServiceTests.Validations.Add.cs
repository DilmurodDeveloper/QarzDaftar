using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            Debt nullDebt = null;
            var nullDebtException = new NullDebtException();

            var expectedDebtValidationException =
                new DebtValidationException(nullDebtException);

            // when
            ValueTask<Debt> addDebtTask =
                this.debtService.AddDebtAsync(nullDebt);

            DebtValidationException actualDebtException =
                await Assert.ThrowsAsync<DebtValidationException>(
                    addDebtTask.AsTask);

            // then
            actualDebtException.Should()
                .BeEquivalentTo(expectedDebtValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDebtAsync(It.IsAny<Debt>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfDebtIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidDebt = new Debt
            {
                Description = invalidText,
            };

            var invalidDebtException = new InvalidDebtException();

            invalidDebtException.AddData(
                key: nameof(Debt.Id),
                values: "Id is required");

            invalidDebtException.AddData(
                key: nameof(Debt.Amount),
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

            invalidDebtException.AddData(
                key: nameof(Debt.UserId),
                values: "Id is required");

            var expectedDebtValidationException =
                new DebtValidationException(invalidDebtException);

            // when
            ValueTask<Debt> addDebtTask =
                this.debtService.AddDebtAsync(invalidDebt);

            DebtValidationException actualDebtValidationException =
                await Assert.ThrowsAsync<DebtValidationException>(
                    addDebtTask.AsTask);

            // then
            actualDebtValidationException.Should()
                .BeEquivalentTo(expectedDebtValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDebtAsync(It.IsAny<Debt>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfStatusIsInvalidAndLogItAsync()
        {
            // given
            Debt randomDebt = CreateRandomDebt();
            Debt invalidDebt = randomDebt;
            invalidDebt.Status = GetInvalidEnum<DebtStatus>();
            var invalidDebtException = new InvalidDebtException();

            invalidDebtException.AddData(
                key: nameof(Debt.Status),
                values: "Value is invalid");

            var expectedDebtValidationException =
                new DebtValidationException(invalidDebtException);

            // when
            ValueTask<Debt> addDebtTask =
                this.debtService.AddDebtAsync(invalidDebt);

            // then
            await Assert.ThrowsAsync<DebtValidationException>(() =>
                addDebtTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDebtAsync(It.IsAny<Debt>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
