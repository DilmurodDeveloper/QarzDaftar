using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;

namespace QarzDaftar.Server.Api.Seeders
{
    public static class SuperAdminSeeder
    {
        public static async Task SeedAsync(
        StorageBroker storageBroker,
        IConfiguration configuration,
        IPasswordHasher<SuperAdmin> passwordHasher)
        {
            var superAdmin = configuration.GetSection("SuperAdmin").Get<SuperAdmin>();

            bool exists = await storageBroker.SuperAdmins.AnyAsync();

            if (!exists)
            {
                superAdmin.PasswordHash = passwordHasher.HashPassword(superAdmin, superAdmin.PasswordHash);

                await storageBroker.InsertSuperAdminAsync(superAdmin);
            }
        }
    }

}
