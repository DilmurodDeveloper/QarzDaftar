using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserNotes
{
    public partial class UserNoteServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveUserNoteByIdAsync()
        {
            // given
            Guid randomUserNoteId = Guid.NewGuid();
            Guid inputUserNoteId = randomUserNoteId;
            UserNote randomUserNote = CreateRandomUserNote();
            UserNote persistedUserNote = randomUserNote;
            UserNote expectedUserNote = persistedUserNote.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserNoteByIdAsync(inputUserNoteId))
                    .ReturnsAsync(persistedUserNote);

            // when
            UserNote actualUserNote =
                await this.userNoteService
                    .RetrieveUserNoteByIdAsync(inputUserNoteId);

            // then 
            actualUserNote.Should().BeEquivalentTo(expectedUserNote);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserNoteByIdAsync(inputUserNoteId),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
