using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Customers;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Customers
{
    public partial class CustomerServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveCustomerByIdAsync()
        {
            // given
            Guid randomCustomerId = Guid.NewGuid();
            Guid inputCustomerId = randomCustomerId;
            Customer randomCustomer = CreateRandomCustomer();
            Customer persistedCustomer = randomCustomer;
            Customer expectedCustomer = persistedCustomer.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomerByIdAsync(inputCustomerId))
                    .ReturnsAsync(persistedCustomer);

            // when
            Customer actualCustomer =
                await this.customerService
                    .RetrieveCustomerByIdAsync(inputCustomerId);

            // then 
            actualCustomer.Should().BeEquivalentTo(expectedCustomer);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(inputCustomerId),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
