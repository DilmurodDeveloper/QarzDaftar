using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllUserPaymentLogs()
        {
            // given
            IQueryable<UserPaymentLog>
                randomUserPaymentLogs =
                    CreateRandomUserPaymentLogs();

            IQueryable<UserPaymentLog>
                persistedUserPaymentLogs =
                    randomUserPaymentLogs;

            IQueryable<UserPaymentLog>
                expectedUserPaymentLogs =
                    persistedUserPaymentLogs.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllUserPaymentLogs())
                    .Returns(persistedUserPaymentLogs);

            // when
            IQueryable<UserPaymentLog>
                actualUserPaymentLogs =
                    this.userPaymentLogService
                        .RetrieveAllUserPaymentLogs();

            // then
            actualUserPaymentLogs.Should()
                .BeEquivalentTo(expectedUserPaymentLogs);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllUserPaymentLogs(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
