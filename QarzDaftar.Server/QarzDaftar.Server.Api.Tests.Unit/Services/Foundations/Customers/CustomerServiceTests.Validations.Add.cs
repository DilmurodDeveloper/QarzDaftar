using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Customers
{
    public partial class CustomerServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            Customer nullCustomer = null;
            var nullCustomerException = new NullCustomerException();

            var expectedCustomerValidationException =
                new CustomerValidationException(nullCustomerException);

            // when
            ValueTask<Customer> addCustomerTask =
                this.customerService.AddCustomerAsync(nullCustomer);

            CustomerValidationException actualCustomerException =
                await Assert.ThrowsAsync<CustomerValidationException>(
                    addCustomerTask.AsTask);

            // then
            actualCustomerException.Should()
                .BeEquivalentTo(expectedCustomerValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCustomerAsync(It.IsAny<Customer>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfCustomerIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidCustomer = new Customer
            {
                FullName = invalidText,
            };

            var invalidCustomerException = new InvalidCustomerException();

            invalidCustomerException.AddData(
                key: nameof(Customer.FullName),
                values: "Text is required");

            invalidCustomerException.AddData(
                key: nameof(Customer.PhoneNumber),
                values: "Text is required");

            invalidCustomerException.AddData(
                key: nameof(Customer.Address),
                values: "Text is required");

            invalidCustomerException.AddData(
                key: nameof(Customer.UserId),
                values: "Id is required");

            var expectedCustomerValidationException =
                new CustomerValidationException(invalidCustomerException);

            // when
            ValueTask<Customer> addCustomerTask =
                this.customerService.AddCustomerAsync(invalidCustomer);

            CustomerValidationException actualCustomerValidationException =
                await Assert.ThrowsAsync<CustomerValidationException>(
                    addCustomerTask.AsTask);

            // then
            actualCustomerValidationException.Should()
                .BeEquivalentTo(expectedCustomerValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertCustomerAsync(It.IsAny<Customer>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
