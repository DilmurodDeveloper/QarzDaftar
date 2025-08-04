using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Payments;
using QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Payments
{
    public partial class PaymentServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid somePaymentId = Guid.NewGuid();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedPaymentException =
                new LockedPaymentException(dbUpdateConcurrencyException);

            var expectedPaymentDependencyValidationException =
                new PaymentDependencyValidationException(lockedPaymentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPaymentByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<Payment> removePaymentById =
                this.paymentService.RemovePaymentByIdAsync(somePaymentId);

            var actualPaymentDependencyValidationException =
                await Assert.ThrowsAsync<PaymentDependencyValidationException>(
                    removePaymentById.AsTask);

            // then
            actualPaymentDependencyValidationException.Should()
                .BeEquivalentTo(expectedPaymentDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPaymentDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeletePaymentAsync(It.IsAny<Payment>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
