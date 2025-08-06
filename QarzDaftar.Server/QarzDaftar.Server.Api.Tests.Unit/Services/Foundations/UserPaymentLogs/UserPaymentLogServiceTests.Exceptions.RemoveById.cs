using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someUserPaymentLogId = Guid.NewGuid();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedUserPaymentLogException =
                new LockedUserPaymentLogException(dbUpdateConcurrencyException);

            var expectedUserPaymentLogDependencyValidationException =
                new UserPaymentLogDependencyValidationException(lockedUserPaymentLogException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserPaymentLogByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<UserPaymentLog> removeUserPaymentLogById =
                this.userPaymentLogService.RemoveUserPaymentLogByIdAsync(someUserPaymentLogId);

            var actualUserPaymentLogDependencyValidationException =
                await Assert.ThrowsAsync<UserPaymentLogDependencyValidationException>(
                    removeUserPaymentLogById.AsTask);

            // then
            actualUserPaymentLogDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserPaymentLogDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserPaymentLogByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserPaymentLogDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteUserPaymentLogAsync(It.IsAny<UserPaymentLog>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
