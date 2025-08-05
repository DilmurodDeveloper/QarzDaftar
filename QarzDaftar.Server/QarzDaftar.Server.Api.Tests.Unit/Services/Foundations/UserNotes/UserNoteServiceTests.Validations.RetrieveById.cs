using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserNotes
{
    public partial class UserNoteServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
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
            ValueTask<UserNote> retrieveUserNoteById =
                this.userNoteService.RetrieveUserNoteByIdAsync(invalidUserNoteId);

            UserNoteValidationException actualUserNoteValidationException =
                await Assert.ThrowsAsync<UserNoteValidationException>(
                    retrieveUserNoteById.AsTask);

            // then
            actualUserNoteValidationException.Should()
                .BeEquivalentTo(expectedUserNoteValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserNoteByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfUserNoteNotFoundAndLogItAsync()
        {
            // given
            Guid someUserNoteId = Guid.NewGuid();
            UserNote noUserNote = null;

            var notFoundUserNoteException =
                new NotFoundUserNoteException(someUserNoteId);

            var expetedUserNoteValidationException =
                new UserNoteValidationException(notFoundUserNoteException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserNoteByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noUserNote);

            // when
            ValueTask<UserNote> retriveByIdUserNoteTask =
                this.userNoteService.RetrieveUserNoteByIdAsync(someUserNoteId);

            UserNoteValidationException actualUserNoteValidationException =
                await Assert.ThrowsAsync<UserNoteValidationException>(
                    retriveByIdUserNoteTask.AsTask);

            // then
            actualUserNoteValidationException.Should()
                .BeEquivalentTo(expetedUserNoteValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserNoteByIdAsync(someUserNoteId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expetedUserNoteValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
