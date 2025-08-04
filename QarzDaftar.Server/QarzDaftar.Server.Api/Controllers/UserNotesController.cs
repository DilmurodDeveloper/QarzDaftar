using Microsoft.AspNetCore.Mvc;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;
using QarzDaftar.Server.Api.Services.Foundations.UserNotes;
using RESTFulSense.Controllers;

namespace QarzDaftar.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserNotesController : RESTFulController
    {
        private readonly IUserNoteService userNoteService;

        public UserNotesController(IUserNoteService userNoteService) =>
            this.userNoteService = userNoteService;

        [HttpPost]
        public async ValueTask<ActionResult<UserNote>> PostUserNoteAsync(UserNote userNote)
        {
            try
            {
                UserNote addUserNote = await this.userNoteService.AddUserNoteAsync(userNote);

                return Created(addUserNote);
            }
            catch (UserNoteValidationException userNoteValidationException)
            {
                return BadRequest(userNoteValidationException.InnerException);
            }
            catch (UserNoteDependencyValidationException userNoteDependencyValidationException)
                when (userNoteDependencyValidationException.InnerException is AlreadyExistsUserNoteException)
            {
                return Conflict(userNoteDependencyValidationException.InnerException);
            }
            catch (UserNoteDependencyException userNoteDependencyException)
            {
                return InternalServerError(userNoteDependencyException.InnerException);
            }
            catch (UserNoteServiceException userNoteServiceException)
            {
                return InternalServerError(userNoteServiceException.InnerException);
            }
        }
    }
}
