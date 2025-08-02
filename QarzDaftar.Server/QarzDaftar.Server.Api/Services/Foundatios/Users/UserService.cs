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

        public async ValueTask<User> RetrieveUserByIdAsync(Guid userId)
        {
            try
            {
                ValidateUserId(userId);

                User maybeUser =
                    await this.storageBroker.SelectUserByIdAsync(userId);

                ValidateStorageUser(maybeUser, userId);

                return maybeUser;
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
        }
    }
}
