using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Customers;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Customers
{
    public partial class CustomerServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllCustomers()
        {
            // given
            Guid someUserId = Guid.NewGuid();
            IQueryable<Customer> randomCustomers = CreateRandomCustomers();
            IQueryable<Customer> persistedCustomers = randomCustomers;
            IQueryable<Customer> expectedCustomers = persistedCustomers.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllCustomers()).Returns(persistedCustomers);

            // when
            IQueryable<Customer> actualCustomers =
                this.customerService.RetrieveAllCustomers(someUserId);

            // then
            actualCustomers.Should().BeEquivalentTo(expectedCustomers);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllCustomers(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
