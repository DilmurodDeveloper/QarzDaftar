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

        [HttpGet("All")]
        public ActionResult<IQueryable<UserNote>> GetAlluserNotes()
        {
            try
            {
                IQueryable<UserNote> alluserNotes =
                    this.userNoteService.RetrieveAllUserNotes();

                return Ok(alluserNotes);
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

        [HttpGet("ById")]
        public async ValueTask<ActionResult<UserNote>> GetUserNoteByIdAsync(Guid userNoteId)
        {
            try
            {
                return await this.userNoteService.RetrieveUserNoteByIdAsync(userNoteId);
            }
            catch (UserNoteDependencyException userNoteDependencyException)
            {
                return InternalServerError(userNoteDependencyException.InnerException);
            }
            catch (UserNoteValidationException userNoteValidationException)
                when (userNoteValidationException.InnerException is InvalidUserNoteException)
            {
                return BadRequest(userNoteValidationException.InnerException);
            }
            catch (UserNoteValidationException userNoteValidationException)
                when (userNoteValidationException.InnerException is NotFoundUserNoteException)
            {
                return NotFound(userNoteValidationException.InnerException);
            }
            catch (UserNoteServiceException userNoteServiceException)
            {
                return InternalServerError(userNoteServiceException.InnerException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<UserNote>> PutUserNoteAsync(UserNote userNote)
        {
            try
            {
                UserNote modifyUserNote =
                    await this.userNoteService.ModifyUserNoteAsync(userNote);

                return Ok(modifyUserNote);
            }
            catch (UserNoteValidationException userNoteValidationException)
                when (userNoteValidationException.InnerException is NotFoundUserNoteException)
            {
                return NotFound(userNoteValidationException.InnerException);
            }
            catch (UserNoteValidationException userNoteValidationException)
            {
                return BadRequest(userNoteValidationException.InnerException);
            }
            catch (UserNoteDependencyValidationException userNoteDependencyValidationException)
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

        [HttpDelete]
        public async ValueTask<ActionResult<UserNote>> DeleteuserNoteAsync(Guid userNoteId)
        {
            try
            {
                UserNote deleteUserNote =
                    await this.userNoteService.RemoveUserNoteByIdAsync(userNoteId);

                return Ok(deleteUserNote);
            }
            catch (UserNoteValidationException userNoteValidationException)
                when (userNoteValidationException.InnerException is NotFoundUserNoteException)
            {
                return NotFound(userNoteValidationException.InnerException);
            }
            catch (UserNoteValidationException userNoteValidationException)
            {
                return BadRequest(userNoteValidationException.InnerException);
            }
            catch (UserNoteDependencyValidationException userNoteDependencyValidationException)
                when (userNoteDependencyValidationException.InnerException is LockedUserNoteException)
            {
                return Locked(userNoteDependencyValidationException.InnerException);
            }
            catch (UserNoteDependencyValidationException userNoteDependencyValidationException)
            {
                return BadRequest(userNoteDependencyValidationException.InnerException);
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
