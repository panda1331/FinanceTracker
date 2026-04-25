using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Application.Interfaces.Infrastructure;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Application.Repository;
using FinanceTracker.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Services
{
    public class AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator) : IAuthService
    {
        public Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (!CheckEmail(request.Email))
                throw new InvalidCredentialsException()
        }

        private bool CheckEmail(string email)
        {
            return true;
        }
    }
}
