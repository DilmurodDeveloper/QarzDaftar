using System.Linq.Expressions;
using Moq;
using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;
using QarzDaftar.Server.Api.Services.Foundations.UserPaymentLogs;
using Tynamix.ObjectFiller;
using Xeptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly IUserPaymentLogService userPaymentLogService;

        public UserPaymentLogServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.userPaymentLogService = new UserPaymentLogService(
                storageBroker: this.storageBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object);
        }

        private static UserPaymentLog CreateRandomUserPaymentLog() =>
            CreateUserPaymentLogFiller(date: GetRandomDateTimeOffset()).Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static Filler<UserPaymentLog> CreateUserPaymentLogFiller(DateTimeOffset date)
        {
            var filler = new Filler<UserPaymentLog>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(date);

            return filler;
        }
    }
}
