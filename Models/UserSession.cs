namespace ERPHub.Models
{
    public class UserSession
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
    }
}
