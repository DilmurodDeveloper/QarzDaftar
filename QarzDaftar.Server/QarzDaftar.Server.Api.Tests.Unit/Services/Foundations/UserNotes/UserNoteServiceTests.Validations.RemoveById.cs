using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserNotes
{
    public partial class UserNoteServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given 
            Guid invalidUserNoteId = Guid.Empty;
            var invalidUserNoteException = new InvalidUserNoteException();

            invalidUserNoteException.AddData(
                key: nameof(UserNote.Id),
                values: "Id is required");

            var expectedUserNoteValidationException =
                new UserNoteValidationException(invalidUserNoteException);

            // when
            ValueTask<UserNote> removeUserNoteById =
                this.userNoteService.RemoveUserNoteByIdAsync(invalidUserNoteId);

            UserNoteValidationException actualUserNoteValidationException =
                await Assert.ThrowsAsync<UserNoteValidationException>(
                    removeUserNoteById.AsTask);
            // then
            actualUserNoteValidationException.Should()
                .BeEquivalentTo(expectedUserNoteValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserNoteByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.storageBrokerMock.Verify(broker =>
            broker.DeleteUserNoteAsync(It.IsAny<UserNote>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveUserNoteByIdIsNotFoundAndLogItAsync()
        {
            // given
            Guid inputUserNoteId = Guid.NewGuid();
            UserNote noUserNote = null;
            var notFoundUserNoteException = new NotFoundUserNoteException(inputUserNoteId);

            var expectedUserNoteValidationException =
                new UserNoteValidationException(notFoundUserNoteException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserNoteByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noUserNote);

            // when
            ValueTask<UserNote> removeUserNoteById =
                this.userNoteService.RemoveUserNoteByIdAsync(inputUserNoteId);

            var actualUserNoteValidationException =
                await Assert.ThrowsAsync<UserNoteValidationException>(
                    removeUserNoteById.AsTask);

            // then
            actualUserNoteValidationException.Should()
                .BeEquivalentTo(expectedUserNoteValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserNoteByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteUserNoteAsync(It.IsAny<UserNote>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
