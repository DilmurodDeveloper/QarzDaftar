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
            var superAdminSection = configuration.GetSection("SuperAdmin");
            var superAdmin = superAdminSection.Get<SuperAdmin>();
            var plainPassword = superAdminSection.GetValue<string>("Password");

            bool exists = await storageBroker.SuperAdmins.AnyAsync();

            if (!exists)
            {
                superAdmin!.PasswordHash = passwordHasher.HashPassword(superAdmin, plainPassword!);

                await storageBroker.InsertSuperAdminAsync(superAdmin);
            }
        }
    }
}
