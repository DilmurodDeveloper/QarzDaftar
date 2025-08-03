using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Customers
{
    public partial class CustomerServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given 
            Guid invalidCustomerId = Guid.Empty;
            var invalidCustomerException = new InvalidCustomerException();

            invalidCustomerException.AddData(
                key: nameof(Customer.Id),
                values: "Id is required");

            var expectedCustomerValidationException =
                new CustomerValidationException(invalidCustomerException);

            // when
            ValueTask<Customer> removeCustomerById =
                this.customerService.RemoveCustomerByIdAsync(invalidCustomerId);

            CustomerValidationException actualCustomerValidationException =
                await Assert.ThrowsAsync<CustomerValidationException>(
                    removeCustomerById.AsTask);
            // then
            actualCustomerValidationException.Should()
                .BeEquivalentTo(expectedCustomerValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.storageBrokerMock.Verify(broker =>
            broker.DeleteCustomerAsync(It.IsAny<Customer>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveCustomerByIdIsNotFoundAndLogItAsync()
        {
            // given
            Guid inputCustomerId = Guid.NewGuid();
            Customer noCustomer = null;
            var notFoundCustomerException = new NotFoundCustomerException(inputCustomerId);

            var expectedCustomerValidationException =
                new CustomerValidationException(notFoundCustomerException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomerByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noCustomer);

            // when
            ValueTask<Customer> removeCustomerById =
                this.customerService.RemoveCustomerByIdAsync(inputCustomerId);

            var actualCustomerValidationException =
                await Assert.ThrowsAsync<CustomerValidationException>(
                    removeCustomerById.AsTask);

            // then
            actualCustomerValidationException.Should()
                .BeEquivalentTo(expectedCustomerValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteCustomerAsync(It.IsAny<Customer>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
