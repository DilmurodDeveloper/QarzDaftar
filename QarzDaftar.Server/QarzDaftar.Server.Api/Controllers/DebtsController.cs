using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;
using QarzDaftar.Server.Api.Services.Processings.Debts;
using RESTFulSense.Controllers;

namespace QarzDaftar.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DebtsController : RESTFulController
    {
        private readonly IDebtProcessingService debtProcessingService;

        public DebtsController(IDebtProcessingService debtProcessingService) =>
            this.debtProcessingService = debtProcessingService;

        [HttpPost]
        public async ValueTask<ActionResult<Debt>> PostDebtAsync(Debt debt)
        {
            try
            {
                Guid userId = GetCurrentUserId();

                Debt addDebt =
                    await this.debtProcessingService.AddDebtAsync(debt, userId);

                return Created(addDebt);
            }
            catch (DebtValidationException debtValidationException)
            {
                return BadRequest(debtValidationException.InnerException);
            }
            catch (DebtDependencyValidationException debtDependencyValidationException)
                when (debtDependencyValidationException.InnerException is AlreadyExistsDebtException)
            {
                return Conflict(debtDependencyValidationException.InnerException);
            }
            catch (DebtDependencyException debtDependencyException)
            {
                return InternalServerError(debtDependencyException.InnerException);
            }
            catch (DebtServiceException debtServiceException)
            {
                return InternalServerError(debtServiceException.InnerException);
            }
        }

        [HttpGet("All")]
        public ActionResult<IQueryable<Debt>> GetAllDebts()
        {
            try
            {
                Guid userId = GetCurrentUserId();

                IQueryable<Debt> allDebts =
                    this.debtProcessingService.RetrieveAllDebtsByUserId(userId);

                return Ok(allDebts);
            }
            catch (DebtDependencyException debtDependencyException)
            {
                return InternalServerError(debtDependencyException.InnerException);
            }
            catch (DebtServiceException debtServiceException)
            {
                return InternalServerError(debtServiceException.InnerException);
            }
        }

        [HttpGet("ById")]
        public async ValueTask<ActionResult<Debt>> GetDebtByIdAsync(Guid debtId)
        {
            try
            {
                Guid userId = GetCurrentUserId();

                Debt maybeDebt =
                    await this.debtProcessingService.RetrieveDebtByIdAsync(debtId);

                if (maybeDebt is null || maybeDebt.Customer?.UserId != userId)
                    return NotFound();

                return Ok(maybeDebt);
            }
            catch (DebtDependencyException debtDependencyException)
            {
                return InternalServerError(debtDependencyException.InnerException);
            }
            catch (DebtValidationException debtValidationException)
                when (debtValidationException.InnerException is InvalidDebtException)
            {
                return BadRequest(debtValidationException.InnerException);
            }
            catch (DebtValidationException debtValidationException)
                when (debtValidationException.InnerException is NotFoundDebtException)
            {
                return NotFound(debtValidationException.InnerException);
            }
            catch (DebtServiceException debtServiceException)
            {
                return InternalServerError(debtServiceException.InnerException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Debt>> PutDebtAsync(Debt debt)
        {
            try
            {
                Guid userId = GetCurrentUserId();

                Debt modifyDebt =
                    await this.debtProcessingService.ModifyDebtAsync(debt, userId);

                return Ok(modifyDebt);
            }
            catch (DebtValidationException debtValidationException)
                when (debtValidationException.InnerException is NotFoundDebtException)
            {
                return NotFound(debtValidationException.InnerException);
            }
            catch (DebtValidationException debtValidationException)
            {
                return BadRequest(debtValidationException.InnerException);
            }
            catch (DebtDependencyValidationException debtDependencyValidationException)
            {
                return Conflict(debtDependencyValidationException.InnerException);
            }
            catch (DebtDependencyException debtDependencyException)
            {
                return InternalServerError(debtDependencyException.InnerException);
            }
            catch (DebtServiceException debtServiceException)
            {
                return InternalServerError(debtServiceException.InnerException);
            }
        }

        [HttpDelete]
        public async ValueTask<ActionResult<Debt>> DeleteDebtAsync(Guid debtId)
        {
            try
            {
                Guid userId = GetCurrentUserId();

                Debt deleteDebt =
                    await this.debtProcessingService.RemoveDebtByIdAsync(debtId, userId);

                return Ok(deleteDebt);
            }
            catch (DebtValidationException debtValidationException)
                when (debtValidationException.InnerException is NotFoundDebtException)
            {
                return NotFound(debtValidationException.InnerException);
            }
            catch (DebtValidationException debtValidationException)
            {
                return BadRequest(debtValidationException.InnerException);
            }
            catch (DebtDependencyValidationException debtDependencyValidationException)
                when (debtDependencyValidationException.InnerException is LockedDebtException)
            {
                return Locked(debtDependencyValidationException.InnerException);
            }
            catch (DebtDependencyValidationException debtDependencyValidationException)
            {
                return BadRequest(debtDependencyValidationException.InnerException);
            }
            catch (DebtDependencyException debtDependencyException)
            {
                return InternalServerError(debtDependencyException.InnerException);
            }
            catch (DebtServiceException debtServiceException)
            {
                return InternalServerError(debtServiceException.InnerException);
            }
        }

        [HttpGet("total/{customerId}")]
        public ActionResult<decimal> GetTotalDebtForCustomer(Guid customerId)
        {
            decimal totalDebt = debtProcessingService.CalculateTotalDebtForCustomer(customerId);
            return Ok(totalDebt);
        }

        [HttpGet("total-debt/user/{userId}")]
        public ActionResult<decimal> GetTotalDebtByUserId(Guid userId)
        {
            var totalDebt = debtProcessingService.CalculateTotalDebtByUserId(userId);
            return Ok(totalDebt);
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
