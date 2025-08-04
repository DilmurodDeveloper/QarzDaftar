using Microsoft.AspNetCore.Mvc;
using QarzDaftar.Server.Api.Models.Foundations.Payments;
using QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions;
using QarzDaftar.Server.Api.Services.Foundations.Payments;
using RESTFulSense.Controllers;

namespace QarzDaftar.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : RESTFulController
    {
        private readonly IPaymentService paymentService;

        public PaymentsController(IPaymentService paymentService) =>
            this.paymentService = paymentService;

        [HttpPost]
        public async ValueTask<ActionResult<Payment>> PostPaymentAsync(Payment payment)
        {
            try
            {
                Payment addPayment = await this.paymentService.AddPaymentAsync(payment);

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
        }

        [HttpGet("All")]
        public ActionResult<IQueryable<Payment>> GetAllPayments()
        {
            try
            {
                IQueryable<Payment> allpayments =
                    this.paymentService.RetrieveAllPayments();

                return Ok(allpayments);
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
                return await this.paymentService.RetrievePaymentByIdAsync(paymentId);
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
    }
}
