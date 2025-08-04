using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserNotes
{
    public partial class UserNoteServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            UserNote nullUserNote = null;
            var nullUserNoteException = new NullUserNoteException();

            var expectedUserNoteValidationException =
                new UserNoteValidationException(nullUserNoteException);

            // when
            ValueTask<UserNote> addUserNoteTask =
                this.userNoteService.AddUserNoteAsync(nullUserNote);

            UserNoteValidationException actualUserNoteException =
                await Assert.ThrowsAsync<UserNoteValidationException>(
                    addUserNoteTask.AsTask);

            // then
            actualUserNoteException.Should()
                .BeEquivalentTo(expectedUserNoteValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserNoteAsync(It.IsAny<UserNote>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfUserNoteIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidUserNote = new UserNote
            {
                Content = invalidText,
            };

            var invalidUserNoteException = new InvalidUserNoteException();

            invalidUserNoteException.AddData(
                key: nameof(UserNote.Id),
                values: "Id is required");

            invalidUserNoteException.AddData(
                key: nameof(UserNote.Content),
                values: "Text is required");

            invalidUserNoteException.AddData(
                key: nameof(UserNote.ReminderDate),
                values: "Date is required");

            invalidUserNoteException.AddData(
                key: nameof(UserNote.CreatedAt),
                values: "Date is required");

            invalidUserNoteException.AddData(
                key: nameof(UserNote.UserId),
                values: "Id is required");

            var expectedUserNoteValidationException =
                new UserNoteValidationException(invalidUserNoteException);

            // when
            ValueTask<UserNote> addUserNoteTask =
                this.userNoteService.AddUserNoteAsync(invalidUserNote);

            UserNoteValidationException actualUserNoteValidationException =
                await Assert.ThrowsAsync<UserNoteValidationException>(
                    addUserNoteTask.AsTask);

            // then
            actualUserNoteValidationException.Should()
                .BeEquivalentTo(expectedUserNoteValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserNoteAsync(It.IsAny<UserNote>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
