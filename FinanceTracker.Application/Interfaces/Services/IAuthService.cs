using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;

namespace FinanceTracker.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
