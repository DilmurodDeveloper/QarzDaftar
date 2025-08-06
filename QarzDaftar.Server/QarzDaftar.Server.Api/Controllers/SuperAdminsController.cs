using Microsoft.AspNetCore.Mvc;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins.Exceptions;
using QarzDaftar.Server.Api.Services.Foundations.SuperAdmins;
using RESTFulSense.Controllers;

namespace QarzDaftar.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuperAdminsController : RESTFulController
    {
        private readonly ISuperAdminService superAdminService;

        public SuperAdminsController(ISuperAdminService superAdminService) =>
            this.superAdminService = superAdminService;

        [HttpGet("Username")]
        public async ValueTask<ActionResult<SuperAdmin>> GetSuperAdminByUsernameAsync(string username)
        {
            try
            {
                return await this.superAdminService.RetrieveSuperAdminByUsernameAsync(username);
            }
            catch (SuperAdminDependencyException superAdminDependencyException)
            {
                return InternalServerError(superAdminDependencyException.InnerException);
            }
            catch (SuperAdminValidationException superAdminValidationException)
                when (superAdminValidationException.InnerException is InvalidSuperAdminException)
            {
                return BadRequest(superAdminValidationException.InnerException);
            }
            catch (SuperAdminValidationException superAdminValidationException)
                when (superAdminValidationException.InnerException is NotFoundSuperAdminException)
            {
                return NotFound(superAdminValidationException.InnerException);
            }
            catch (SuperAdminServiceException superAdminServiceException)
            {
                return InternalServerError(superAdminServiceException.InnerException);
            }
        }
    }
}
