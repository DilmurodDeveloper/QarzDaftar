using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Customers;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Customers
{
    public partial class CustomerServiceTests
    {
        [Fact]
        public async Task ShouldModifyCustomerAsync()
        {
            // given 
            Customer randomCustomer = CreateRandomCustomer();
            Customer inputCustomer = randomCustomer;
            Customer persistedCustomer = inputCustomer.DeepClone();
            Customer updatedCustomer = inputCustomer;
            Customer expectedCustomer = updatedCustomer.DeepClone();
            Guid InputCustomerId = inputCustomer.Id;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomerByIdAsync(InputCustomerId))
                    .ReturnsAsync(persistedCustomer);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateCustomerAsync(inputCustomer))
                    .ReturnsAsync(updatedCustomer);

            // when
            Customer actualCustomer =
                await this.customerService.ModifyCustomerAsync(inputCustomer);

            // then
            actualCustomer.Should().BeEquivalentTo(expectedCustomer);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(InputCustomerId), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomerAsync(inputCustomer), Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
