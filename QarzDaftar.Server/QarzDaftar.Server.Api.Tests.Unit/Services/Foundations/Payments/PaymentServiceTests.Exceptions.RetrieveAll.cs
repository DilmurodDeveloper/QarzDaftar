using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Payments
{
    public partial class PaymentServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given 
            SqlException sqlException = GetSqlError();

            var failedPaymentStorageException =
                new FailedPaymentStorageException(sqlException);

            var expectedPaymentDependencyException =
                new PaymentDependencyException(failedPaymentStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllPayments()).Throws(sqlException);

            // when
            Action retrieveAllPaymentsAction = () =>
                this.paymentService.RetrieveAllPayments();

            PaymentDependencyException actualPaymentDependencyException =
                Assert.Throws<PaymentDependencyException>(retrieveAllPaymentsAction);

            // then
            actualPaymentDependencyException.Should()
                .BeEquivalentTo(expectedPaymentDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllPayments(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedPaymentDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
