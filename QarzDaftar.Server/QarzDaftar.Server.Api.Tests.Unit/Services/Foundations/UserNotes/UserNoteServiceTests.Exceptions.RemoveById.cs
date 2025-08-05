using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserNotes
{
    public partial class UserNoteServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someUserNoteId = Guid.NewGuid();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedUserNoteException =
                new LockedUserNoteException(dbUpdateConcurrencyException);

            var expectedUserNoteDependencyValidationException =
                new UserNoteDependencyValidationException(lockedUserNoteException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserNoteByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<UserNote> removeUserNoteById =
                this.userNoteService.RemoveUserNoteByIdAsync(someUserNoteId);

            var actualUserNoteDependencyValidationException =
                await Assert.ThrowsAsync<UserNoteDependencyValidationException>(
                    removeUserNoteById.AsTask);

            // then
            actualUserNoteDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserNoteDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserNoteByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteUserNoteAsync(It.IsAny<UserNote>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
