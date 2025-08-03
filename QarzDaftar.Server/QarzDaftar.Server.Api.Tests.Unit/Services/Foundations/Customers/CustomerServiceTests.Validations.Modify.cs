using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Customers
{
    public partial class CustomerServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCustomerIsNullAndLogItAsync()
        {
            // given
            Customer nullCustomer = null;
            var nullCustomerException = new NullCustomerException();

            var expectedCustomerValidationException =
                new CustomerValidationException(nullCustomerException);

            // when
            ValueTask<Customer> modifyCustomerTask =
                this.customerService.ModifyCustomerAsync(nullCustomer);

            CustomerValidationException actualCustomerValidationException =
                await Assert.ThrowsAsync<CustomerValidationException>(
                    modifyCustomerTask.AsTask);

            // then
            actualCustomerValidationException.Should()
                .BeEquivalentTo(expectedCustomerValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomerAsync(It.IsAny<Customer>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfCustomerIsInvalidAndLogItAsync(
            string invalidString)
        {
            //given
            Customer invalidCustomer = new Customer
            {
                FullName = invalidString
            };

            var invalidCustomerException = new InvalidCustomerException();

            invalidCustomerException.AddData(
                key: nameof(Customer.Id),
                values: "Id is required");

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
                key: nameof(Customer.CreatedDate),
                values: "Date is required");

            invalidCustomerException.AddData(
                key: nameof(Customer.UpdatedDate),
                values: "Date is required");

            invalidCustomerException.AddData(
                key: nameof(Customer.UserId),
                values: "Id is required");

            var expectedCustomerValidationException =
                new CustomerValidationException(invalidCustomerException);

            // when
            ValueTask<Customer> modifyCustomerTask =
                this.customerService.ModifyCustomerAsync(invalidCustomer);

            CustomerValidationException actualCustomerValidationException =
                await Assert.ThrowsAsync<CustomerValidationException>(
                    modifyCustomerTask.AsTask);

            // then
            actualCustomerValidationException.Should()
                .BeEquivalentTo(expectedCustomerValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomerAsync(It.IsAny<Customer>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCustomerDoesNotExistAndLogItAsync()
        {
            // given
            Customer randomCustomer = CreateRandomCustomer();
            Customer nonExistCustomer = randomCustomer;
            Customer nullCustomer = null;

            var notFoundCustomerException =
                new NotFoundCustomerException(nonExistCustomer.Id);

            var expectedCustomerValidationException =
                new CustomerValidationException(notFoundCustomerException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectCustomerByIdAsync(nonExistCustomer.Id))
                    .ReturnsAsync(nullCustomer);

            // when
            ValueTask<Customer> modifyCustomerTask =
                this.customerService.ModifyCustomerAsync(nonExistCustomer);

            CustomerValidationException actualCustomerValidationException =
                await Assert.ThrowsAsync<CustomerValidationException>
                    (modifyCustomerTask.AsTask);

            // then
            actualCustomerValidationException.Should()
                .BeEquivalentTo(expectedCustomerValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectCustomerByIdAsync(nonExistCustomer.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedCustomerValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateCustomerAsync(nonExistCustomer), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
