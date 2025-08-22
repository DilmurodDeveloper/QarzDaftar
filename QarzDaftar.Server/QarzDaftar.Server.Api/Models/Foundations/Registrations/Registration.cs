namespace QarzDaftar.Server.Api.Models.Foundations.Registrations
{
    public class Registration
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    }
}
