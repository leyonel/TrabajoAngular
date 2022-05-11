namespace Models.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public string Token { get; set; } = string.Empty;
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
