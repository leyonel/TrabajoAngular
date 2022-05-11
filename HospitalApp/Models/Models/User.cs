namespace Models.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
    }
}
