using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QarzDaftar.Server.Api.Models.Foundations.Payments;
using QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions;
using QarzDaftar.Server.Api.Services.Processings.Payments;
using RESTFulSense.Controllers;

namespace QarzDaftar.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class PaymentsController : RESTFulController
    {
        private readonly IPaymentProcessingService paymentProcessingService;

        public PaymentsController(IPaymentProcessingService paymentProcessingService) =>
            this.paymentProcessingService = paymentProcessingService;

        [HttpPost]
        public async ValueTask<ActionResult<Payment>> PostPaymentAsync(Payment payment)
        {
            try
            {
                Guid userId = GetCurrentUserId();

                Payment addPayment = await this.paymentProcessingService.AddPaymentAsync(payment, userId);

                return Created(addPayment);
            }
            catch (PaymentValidationException paymentValidationException)
            {
                return BadRequest(paymentValidationException.InnerException);
            }
            catch (PaymentDependencyValidationException paymentDependencyValidationException)
                when (paymentDependencyValidationException.InnerException is AlreadyExistsPaymentException)
            {
                return Conflict(paymentDependencyValidationException.InnerException);
            }
            catch (PaymentDependencyException paymentDependencyException)
            {
                return InternalServerError(paymentDependencyException.InnerException);
            }
            catch (PaymentServiceException paymentServiceException)
            {
                return InternalServerError(paymentServiceException.InnerException);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpGet("All")]
        public ActionResult<IQueryable<Payment>> GetAllPayments()
        {
            try
            {
                Guid userId = GetCurrentUserId();

                IQueryable<Payment> allPayments =
                    this.paymentProcessingService.RetrieveAllPaymentsByUserId(userId);

                return Ok(allPayments);
            }
            catch (PaymentDependencyException paymentDependencyException)
            {
                return InternalServerError(paymentDependencyException.InnerException);
            }
            catch (PaymentServiceException paymentServiceException)
            {
                return InternalServerError(paymentServiceException.InnerException);
            }
        }

        [HttpGet("ById")]
        public async ValueTask<ActionResult<Payment>> GetPaymentByIdAsync(Guid paymentId)
        {
            try
            {
                Guid userId = GetCurrentUserId();

                Payment maybePayment =
                    await this.paymentProcessingService.RetrievePaymentByIdAsync(paymentId);

                if (maybePayment == null || maybePayment.Customer?.UserId != userId)
                    return NotFound();

                return Ok(maybePayment);
            }
            catch (PaymentDependencyException paymentDependencyException)
            {
                return InternalServerError(paymentDependencyException.InnerException);
            }
            catch (PaymentValidationException paymentValidationException)
                when (paymentValidationException.InnerException is InvalidPaymentException)
            {
                return BadRequest(paymentValidationException.InnerException);
            }
            catch (PaymentValidationException paymentValidationException)
                when (paymentValidationException.InnerException is NotFoundPaymentException)
            {
                return NotFound(paymentValidationException.InnerException);
            }
            catch (PaymentServiceException paymentServiceException)
            {
                return InternalServerError(paymentServiceException.InnerException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Payment>> PutPaymentAsync(Payment payment)
        {
            try
            {
                Guid userId = GetCurrentUserId();

                Payment modifyPayment =
                    await this.paymentProcessingService.ModifyPaymentAsync(payment, userId);

                return Ok(modifyPayment);
            }
            catch (PaymentValidationException paymentValidationException)
                when (paymentValidationException.InnerException is NotFoundPaymentException)
            {
                return NotFound(paymentValidationException.InnerException);
            }
            catch (PaymentValidationException paymentValidationException)
            {
                return BadRequest(paymentValidationException.InnerException);
            }
            catch (PaymentDependencyValidationException paymentDependencyValidationException)
            {
                return Conflict(paymentDependencyValidationException.InnerException);
            }
            catch (PaymentDependencyException paymentDependencyException)
            {
                return InternalServerError(paymentDependencyException.InnerException);
            }
            catch (PaymentServiceException paymentServiceException)
            {
                return InternalServerError(paymentServiceException.InnerException);
            }
        }

        [HttpDelete]
        public async ValueTask<ActionResult<Payment>> DeletePaymentAsync(Guid paymentId)
        {
            try
            {
                Guid userId = GetCurrentUserId();

                Payment deletePayment =
                    await this.paymentProcessingService.RemovePaymentByIdAsync(paymentId, userId);

                return Ok(deletePayment);
            }
            catch (PaymentValidationException paymentValidationException)
                when (paymentValidationException.InnerException is NotFoundPaymentException)
            {
                return NotFound(paymentValidationException.InnerException);
            }
            catch (PaymentValidationException paymentValidationException)
            {
                return BadRequest(paymentValidationException.InnerException);
            }
            catch (PaymentDependencyValidationException paymentDependencyValidationException)
                when (paymentDependencyValidationException.InnerException is LockedPaymentException)
            {
                return Locked(paymentDependencyValidationException.InnerException);
            }
            catch (PaymentDependencyValidationException paymentDependencyValidationException)
            {
                return BadRequest(paymentDependencyValidationException.InnerException);
            }
            catch (PaymentDependencyException paymentDependencyException)
            {
                return InternalServerError(paymentDependencyException.InnerException);
            }
            catch (PaymentServiceException paymentServiceException)
            {
                return InternalServerError(paymentServiceException.InnerException);
            }
        }

        [HttpGet("customers/{customerId}/total-paid")]
        public ActionResult<decimal> GetTotalPaidAmountForCustomer(Guid customerId)
        {
            decimal totalPaid = paymentProcessingService.CalculateTotalPaidAmountForCustomer(customerId);
            return Ok(totalPaid);
        }

        [HttpGet("customers/{customerId}/remaining-debt")]
        public ActionResult<decimal> GetRemainingDebtForCustomer(Guid customerId)
        {
            decimal remainingDebt = paymentProcessingService.CalculateRemainingDebtForCustomer(customerId);
            return Ok(remainingDebt);
        }

        [HttpGet("total-paid/user/{userId}")]
        public ActionResult<decimal> GetTotalPaidByUserId(Guid userId)
        {
            var totalPaid = paymentProcessingService.CalculateTotalPaidByUserId(userId);
            return Ok(totalPaid);
        }

        [HttpGet("remaining-debt/user/{userId}")]
        public ActionResult<decimal> GetRemainingDebtByUserId(Guid userId)
        {
            var remainingDebt = paymentProcessingService.CalculateRemainingDebtByUserId(userId);
            return Ok(remainingDebt);
        }

        private Guid GetCurrentUserId()
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }

            return userId;
        }
    }
}
