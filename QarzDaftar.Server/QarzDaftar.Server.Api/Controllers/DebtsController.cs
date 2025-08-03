using Microsoft.AspNetCore.Mvc;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;
using QarzDaftar.Server.Api.Services.Foundations.Debts;
using RESTFulSense.Controllers;

namespace QarzDaftar.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebtsController : RESTFulController
    {
        private readonly IDebtService debtService;

        public DebtsController(IDebtService debtService) =>
            this.debtService = debtService;

        [HttpPost]
        public async ValueTask<ActionResult<Debt>> PostDebtAsync(Debt debt)
        {
            try
            {
                Debt addDebt = await this.debtService.AddDebtAsync(debt);

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
                IQueryable<Debt> allDebts = this.debtService.RetrieveAllDebts();

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
                return await this.debtService.RetrieveDebtByIdAsync(debtId);
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
                Debt modifyDebt =
                    await this.debtService.ModifyDebtAsync(debt);

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
    }
}
