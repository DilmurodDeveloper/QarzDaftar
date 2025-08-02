using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.Users;
using QarzDaftar.Server.Api.Models.Foundations.Users.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundatios.Users
{
    public partial class UserService : IUserService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public UserService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<User> AddUserAsync(User user) =>
        TryCatch(async () =>
        {
            ValidateUserOnAdd(user);

            return await this.storageBroker.InsertUserAsync(user);
        });

        public IQueryable<User> RetrieveAllUsers() =>
            TryCatch(() => this.storageBroker.SelectAllUsers());

        public ValueTask<User> RetrieveUserByIdAsync(Guid userId) =>
        TryCatch(async () =>
        {
            ValidateUserId(userId);

            User maybeUser =
                await this.storageBroker.SelectUserByIdAsync(userId);

            ValidateStorageUser(maybeUser, userId);

            return maybeUser;
        });

        public async ValueTask<User> ModifyUserAsync(User user)
        {
            try
            {
                ValidateUserOnModify(user);

                User maybeUser =
                    await this.storageBroker.SelectUserByIdAsync(user.Id);

                ValidateAgainstStorageUserOnModify(user, maybeUser);

                return await this.storageBroker.UpdateUserAsync(user);
            }
            catch (NullUserException nullUserException)
            {
                var userValidationException =
                    new UserValidationException(nullUserException);

                this.loggingBroker.LogError(userValidationException);

                throw userValidationException;
            }
            catch (InvalidUserException invalidUserException)
            {
                var userValidationException =
                    new UserValidationException(invalidUserException);

                this.loggingBroker.LogError(userValidationException);

                throw userValidationException;
            }
            catch (NotFoundUserException notFoundUserException)
            {
                var userValidationException =
                    new UserValidationException(notFoundUserException);

                this.loggingBroker.LogError(userValidationException);

                throw userValidationException;
            }
            catch (SqlException sqlException)
            {
                var failedUserStorageException =
                    new FailedUserStorageException(sqlException);

                var userDependencyException =
                    new UserDependencyException(failedUserStorageException);

                this.loggingBroker.LogCritical(userDependencyException);

                throw userDependencyException;
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedUserException =
                    new LockedUserException(dbUpdateConcurrencyException);

                var userDependencyValidationException =
                    new UserDependencyValidationException(lockedUserException);

                this.loggingBroker.LogError(userDependencyValidationException);

                throw userDependencyValidationException;
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedUserStorageException =
                    new FailedUserStorageException(dbUpdateException);

                var userDependencyException =
                    new UserDependencyException(failedUserStorageException);

                this.loggingBroker.LogError(userDependencyException);

                throw userDependencyException;
            }
            catch (Exception exception)
            {
                var failedUserServiceException =
                    new FailedUserServiceException(exception);

                var userServiceException =
                    new UserServiceException(failedUserServiceException);

                this.loggingBroker.LogError(userServiceException);

                throw userServiceException;
            }
        }
    }
}
