using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserNotes
{
    public partial class UserNoteServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllUserNotes()
        {
            // given
            IQueryable<UserNote> randomUserNotes = CreateRandomUserNotes();
            IQueryable<UserNote> persistedUserNotes = randomUserNotes;
            IQueryable<UserNote> expectedUserNotes = persistedUserNotes.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllUserNotes()).Returns(persistedUserNotes);

            // when
            IQueryable<UserNote> actualUserNotes =
                this.userNoteService.RetrieveAllUserNotes();

            // then
            actualUserNotes.Should().BeEquivalentTo(expectedUserNotes);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllUserNotes(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
