using Microsoft.AspNetCore.Mvc;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions;
using QarzDaftar.Server.Api.Services.Foundations.SubscriptionHistories;
using RESTFulSense.Controllers;

namespace QarzDaftar.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionHistoriesController : RESTFulController
    {
        private readonly ISubscriptionHistoryService subscriptionHistoryService;

        public SubscriptionHistoriesController(
            ISubscriptionHistoryService subscriptionHistoryService) =>
                this.subscriptionHistoryService = subscriptionHistoryService;

        [HttpPost]
        public async ValueTask<ActionResult<SubscriptionHistory>> PostSubscriptionHistoryAsync(
            SubscriptionHistory subscriptionHistory)
        {
            try
            {
                SubscriptionHistory addSubscriptionHistory =
                    await this.subscriptionHistoryService.AddSubscriptionHistoryAsync(
                        subscriptionHistory);

                return Created(addSubscriptionHistory);
            }
            catch (SubscriptionHistoryValidationException subscriptionHistoryValidationException)
            {
                return BadRequest(subscriptionHistoryValidationException.InnerException);
            }
            catch (SubscriptionHistoryDependencyValidationException subscriptionHistoryDependencyValidationException)
                when (subscriptionHistoryDependencyValidationException.InnerException is AlreadyExistsSubscriptionHistoryException)
            {
                return Conflict(subscriptionHistoryDependencyValidationException.InnerException);
            }
            catch (SubscriptionHistoryDependencyException subscriptionHistoryDependencyException)
            {
                return InternalServerError(subscriptionHistoryDependencyException.InnerException);
            }
            catch (SubscriptionHistoryServiceException subscriptionHistoryServiceException)
            {
                return InternalServerError(subscriptionHistoryServiceException.InnerException);
            }
        }

        [HttpGet("All")]
        public ActionResult<IQueryable<SubscriptionHistory>> GetAllSubscriptionHistorys()
        {
            try
            {
                IQueryable<SubscriptionHistory> allSubscriptionHistorys =
                    this.subscriptionHistoryService.RetrieveAllSubscriptionHistories();

                return Ok(allSubscriptionHistorys);
            }
            catch (SubscriptionHistoryDependencyException subscriptionHistoryDependencyException)
            {
                return InternalServerError(subscriptionHistoryDependencyException.InnerException);
            }
            catch (SubscriptionHistoryServiceException subscriptionHistoryServiceException)
            {
                return InternalServerError(subscriptionHistoryServiceException.InnerException);
            }
        }

        [HttpGet("ById")]
        public async ValueTask<ActionResult<SubscriptionHistory>> GetSubscriptionHistoryByIdAsync(Guid subscriptionHistoryId)
        {
            try
            {
                return await this.subscriptionHistoryService.RetrieveSubscriptionHistoryByIdAsync(subscriptionHistoryId);
            }
            catch (SubscriptionHistoryDependencyException subscriptionHistoryDependencyException)
            {
                return InternalServerError(subscriptionHistoryDependencyException.InnerException);
            }
            catch (SubscriptionHistoryValidationException subscriptionHistoryValidationException)
                when (subscriptionHistoryValidationException.InnerException is InvalidSubscriptionHistoryException)
            {
                return BadRequest(subscriptionHistoryValidationException.InnerException);
            }
            catch (SubscriptionHistoryValidationException subscriptionHistoryValidationException)
                when (subscriptionHistoryValidationException.InnerException is NotFoundSubscriptionHistoryException)
            {
                return NotFound(subscriptionHistoryValidationException.InnerException);
            }
            catch (SubscriptionHistoryServiceException subscriptionHistoryServiceException)
            {
                return InternalServerError(subscriptionHistoryServiceException.InnerException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<SubscriptionHistory>> PutSubscriptionHistoryAsync(SubscriptionHistory subscriptionHistory)
        {
            try
            {
                SubscriptionHistory modifySubscriptionHistory =
                    await this.subscriptionHistoryService.ModifySubscriptionHistoryAsync(subscriptionHistory);

                return Ok(modifySubscriptionHistory);
            }
            catch (SubscriptionHistoryValidationException subscriptionHistoryValidationException)
                when (subscriptionHistoryValidationException.InnerException is NotFoundSubscriptionHistoryException)
            {
                return NotFound(subscriptionHistoryValidationException.InnerException);
            }
            catch (SubscriptionHistoryValidationException subscriptionHistoryValidationException)
            {
                return BadRequest(subscriptionHistoryValidationException.InnerException);
            }
            catch (SubscriptionHistoryDependencyValidationException subscriptionHistoryDependencyValidationException)
            {
                return Conflict(subscriptionHistoryDependencyValidationException.InnerException);
            }
            catch (SubscriptionHistoryDependencyException subscriptionHistoryDependencyException)
            {
                return InternalServerError(subscriptionHistoryDependencyException.InnerException);
            }
            catch (SubscriptionHistoryServiceException subscriptionHistoryServiceException)
            {
                return InternalServerError(subscriptionHistoryServiceException.InnerException);
            }
        }

        [HttpDelete]
        public async ValueTask<ActionResult<SubscriptionHistory>> DeletesubscriptionHistoryAsync(Guid subscriptionHistoryId)
        {
            try
            {
                SubscriptionHistory deleteSubscriptionHistory =
                    await this.subscriptionHistoryService.RemoveSubscriptionHistoryByIdAsync(subscriptionHistoryId);

                return Ok(deleteSubscriptionHistory);
            }
            catch (SubscriptionHistoryValidationException subscriptionHistoryValidationException)
                when (subscriptionHistoryValidationException.InnerException is NotFoundSubscriptionHistoryException)
            {
                return NotFound(subscriptionHistoryValidationException.InnerException);
            }
            catch (SubscriptionHistoryValidationException subscriptionHistoryValidationException)
            {
                return BadRequest(subscriptionHistoryValidationException.InnerException);
            }
            catch (SubscriptionHistoryDependencyValidationException subscriptionHistoryDependencyValidationException)
                when (subscriptionHistoryDependencyValidationException.InnerException is LockedSubscriptionHistoryException)
            {
                return Locked(subscriptionHistoryDependencyValidationException.InnerException);
            }
            catch (SubscriptionHistoryDependencyValidationException subscriptionHistoryDependencyValidationException)
            {
                return BadRequest(subscriptionHistoryDependencyValidationException.InnerException);
            }
            catch (SubscriptionHistoryDependencyException subscriptionHistoryDependencyException)
            {
                return InternalServerError(subscriptionHistoryDependencyException.InnerException);
            }
            catch (SubscriptionHistoryServiceException subscriptionHistoryServiceException)
            {
                return InternalServerError(subscriptionHistoryServiceException.InnerException);
            }
        }
    }
}
