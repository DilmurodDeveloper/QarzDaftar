namespace QarzDaftar.Server.Api.Models.Foundations.SuperAdmins
{
    public class SuperAdmin
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string Role { get; set; } = "SuperAdmin";
    }
}
