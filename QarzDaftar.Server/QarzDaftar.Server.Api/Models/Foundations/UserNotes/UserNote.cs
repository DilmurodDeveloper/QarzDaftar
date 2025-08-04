using System.Text.Json.Serialization;
using QarzDaftar.Server.Api.Models.Enums;
using QarzDaftar.Server.Api.Models.Foundations.Users;

namespace QarzDaftar.Server.Api.Models.Foundations.UserNotes
{
    public class UserNote
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTimeOffset ReminderDate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public ReminderStatus Status { get; set; }
        public Guid UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }
    }
}
