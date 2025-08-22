using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QarzDaftar.Server.Api.Models.Foundations.Registrations;
using QarzDaftar.Server.Api.Services.Foundations.Registrations;
using RESTFulSense.Controllers;

namespace QarzDaftar.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationsController : RESTFulController
    {
        private readonly IRegistrationService registrationService;

        public RegistrationsController(IRegistrationService registrationService) =>
            this.registrationService = registrationService;

        [HttpPost]
        public async ValueTask<ActionResult> PostRegistrationAsync(Registration registration)
        {
            Registration addedRegistration =
                await this.registrationService.AddRegistrationAsync(registration);

            return Created(addedRegistration);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult<IEnumerable<Registration>> GetAllRegistrations()
        {
            IEnumerable<Registration> registrations =
                this.registrationService.RetrieveAllRegistrations().ToList();

            return Ok(registrations);
        }
    }
}