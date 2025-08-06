using Microsoft.AspNetCore.Mvc;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions;
using QarzDaftar.Server.Api.Services.Foundations.UserPaymentLogs;
using RESTFulSense.Controllers;

namespace QarzDaftar.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPaymentLogsController : RESTFulController
    {
        private readonly IUserPaymentLogService userPaymentLogService;

        public UserPaymentLogsController(IUserPaymentLogService userPaymentLogService) =>
            this.userPaymentLogService = userPaymentLogService;

        [HttpPost]
        public async ValueTask<ActionResult<UserPaymentLog>> PostUserPaymentLogAsync(UserPaymentLog userPaymentLog)
        {
            try
            {
                UserPaymentLog addUserPaymentLog =
                    await this.userPaymentLogService.AddUserPaymentLogAsync(userPaymentLog);

                return Created(addUserPaymentLog);
            }
            catch (UserPaymentLogValidationException userPaymentLogValidationException)
            {
                return BadRequest(userPaymentLogValidationException.InnerException);
            }
            catch (UserPaymentLogDependencyValidationException userPaymentLogDependencyValidationException)
                when (userPaymentLogDependencyValidationException.InnerException is AlreadyExistsUserPaymentLogException)
            {
                return Conflict(userPaymentLogDependencyValidationException.InnerException);
            }
            catch (UserPaymentLogDependencyException userPaymentLogDependencyException)
            {
                return InternalServerError(userPaymentLogDependencyException.InnerException);
            }
            catch (UserPaymentLogServiceException userPaymentLogServiceException)
            {
                return InternalServerError(userPaymentLogServiceException.InnerException);
            }
        }

        [HttpGet("All")]
        public ActionResult<IQueryable<UserPaymentLog>> GetAllUserPaymentLogs()
        {
            try
            {
                IQueryable<UserPaymentLog> allUserPaymentLogs =
                    this.userPaymentLogService.RetrieveAllUserPaymentLogs();

                return Ok(allUserPaymentLogs);
            }
            catch (UserPaymentLogDependencyException userPaymentLogDependencyException)
            {
                return InternalServerError(userPaymentLogDependencyException.InnerException);
            }
            catch (UserPaymentLogServiceException userPaymentLogServiceException)
            {
                return InternalServerError(userPaymentLogServiceException.InnerException);
            }
        }

        [HttpGet("ById")]
        public async ValueTask<ActionResult<UserPaymentLog>> GetUserPaymentLogByIdAsync(Guid userPaymentLogId)
        {
            try
            {
                return await this.userPaymentLogService.RetrieveUserPaymentLogByIdAsync(userPaymentLogId);
            }
            catch (UserPaymentLogDependencyException userPaymentLogDependencyException)
            {
                return InternalServerError(userPaymentLogDependencyException.InnerException);
            }
            catch (UserPaymentLogValidationException userPaymentLogValidationException)
                when (userPaymentLogValidationException.InnerException is InvalidUserPaymentLogException)
            {
                return BadRequest(userPaymentLogValidationException.InnerException);
            }
            catch (UserPaymentLogValidationException userPaymentLogValidationException)
                when (userPaymentLogValidationException.InnerException is NotFoundUserPaymentLogException)
            {
                return NotFound(userPaymentLogValidationException.InnerException);
            }
            catch (UserPaymentLogServiceException userPaymentLogServiceException)
            {
                return InternalServerError(userPaymentLogServiceException.InnerException);
            }
        }
    }
}
