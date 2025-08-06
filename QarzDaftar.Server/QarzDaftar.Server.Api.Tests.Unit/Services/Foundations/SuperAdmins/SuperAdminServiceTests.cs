using Moq;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;
using QarzDaftar.Server.Api.Services.Foundations.SuperAdmins;
using Tynamix.ObjectFiller;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.SuperAdmins
{
    public partial class SuperAdminServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ISuperAdminService superAdminService;

        public SuperAdminServiceTests()
        {
            storageBrokerMock = new Mock<IStorageBroker>();
            loggingBrokerMock = new Mock<ILoggingBroker>();
            superAdminService = new SuperAdminService(
                storageBroker: storageBrokerMock.Object,
                loggingBroker: loggingBrokerMock.Object);
        }

        private static SuperAdmin CreateRandomSuperAdmin() =>
            CreateSuperAdminFiller(date: GetRandomDateTimeOffset()).Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static string GetRandomUsername() => Guid.NewGuid().ToString();

        private static Filler<SuperAdmin> CreateSuperAdminFiller(DateTimeOffset date)
        {
            var filler = new Filler<SuperAdmin>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(date);

            return filler;
        }
    }
}
