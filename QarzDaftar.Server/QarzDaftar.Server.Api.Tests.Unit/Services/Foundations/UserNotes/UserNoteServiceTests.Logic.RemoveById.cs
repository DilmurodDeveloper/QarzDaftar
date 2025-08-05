using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserNotes
{
    public partial class UserNoteServiceTests
    {
        [Fact]
        public async Task ShouldRemoveUserNoteByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputUserNoteId = randomId;
            UserNote randomUserNote = CreateRandomUserNote();
            UserNote storageUserNote = randomUserNote;
            UserNote expectedInputUserNote = storageUserNote;
            UserNote deletedUserNote = expectedInputUserNote;
            UserNote expectedUserNote = deletedUserNote.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserNoteByIdAsync(inputUserNoteId))
                    .ReturnsAsync(storageUserNote);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteUserNoteAsync(expectedInputUserNote))
                    .ReturnsAsync(deletedUserNote);

            // when
            UserNote actualUserNote =
                await this.userNoteService.RemoveUserNoteByIdAsync(randomId);

            // then 
            actualUserNote.Should().BeEquivalentTo(expectedUserNote);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserNoteByIdAsync(inputUserNoteId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteUserNoteAsync(expectedInputUserNote),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
