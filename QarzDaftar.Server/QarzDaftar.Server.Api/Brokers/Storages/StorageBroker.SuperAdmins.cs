using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<SuperAdmin> SuperAdmins { get; set; }

        public async ValueTask<SuperAdmin> InsertSuperAdminAsync(SuperAdmin superAdmin) =>
            await InsertAsync(superAdmin);

        public async ValueTask<SuperAdmin> SelectSuperAdminByUsernameAsync(string username) =>
            await this.SuperAdmins
                .AsNoTracking()
                .FirstOrDefaultAsync(admin => admin.Username == username);
    }
}
