﻿using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<UserNote> UserNotes { get; set; }
    }
}
