namespace FinanceTracker.Application.DTOs.Responses
{
    public class AuthResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
