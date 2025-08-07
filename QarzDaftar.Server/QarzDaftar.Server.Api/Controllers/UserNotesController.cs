using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;
using QarzDaftar.Server.Api.Services.Processings.UserNotes;
using RESTFulSense.Controllers;

namespace QarzDaftar.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserNotesController : RESTFulController
    {
        private readonly IUserNoteProcessingService userNoteProcessingService;

        public UserNotesController(IUserNoteProcessingService userNoteProcessingService) =>
            this.userNoteProcessingService = userNoteProcessingService;

        [HttpPost]
        public async ValueTask<ActionResult<UserNote>> PostUserNoteAsync(UserNote userNote)
        {
            try
            {
                Guid userId = GetCurrentUserId();

                UserNote addedUserNote =
                    await this.userNoteProcessingService.AddUserNoteAsync(userNote, userId);

                return Created(addedUserNote);
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
                Guid userId = GetCurrentUserId();

                IQueryable<UserNote> allUserNotes =
                    this.userNoteProcessingService.RetrieveAllUserNotesByUserId(userId);

                return Ok(allUserNotes);
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
                Guid userId = GetCurrentUserId();

                UserNote maybeUserNote =
                    await this.userNoteProcessingService.RetrieveUserNoteByIdAsync(userNoteId);

                if (maybeUserNote == null || maybeUserNote.UserId != userId)
                    return NotFound();

                return Ok(maybeUserNote);
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
                Guid userId = GetCurrentUserId();

                UserNote modifiedUserNote =
                    await this.userNoteProcessingService.ModifyUserNoteAsync(userNote, userId);

                return Ok(modifiedUserNote);
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
                Guid userId = GetCurrentUserId();

                UserNote deletedUserNote =
                    await this.userNoteProcessingService.RemoveUserNoteByIdAsync(userNoteId, userId);

                return Ok(deletedUserNote);
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

        private Guid GetCurrentUserId()
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }

            return userId;
        }
    }
}
