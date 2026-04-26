using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Application.Interfaces.Infrastructure;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Application.Repository;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using FinanceTracker.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace FinanceTracker.Application.Services
{
    public class AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator) : IAuthService
    {
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                throw new InvalidCredentialsException("Invalid email or password");

            var isCorrectPassword = passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

            if (!isCorrectPassword)
                throw new InvalidCredentialsException("Invalid email or password");

            var token = tokenGenerator.GenerateToken(user.Id, user.Email, user.Type);
            return new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Token = token,
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (!CheckEmail(request.Email))
                throw new InvalidCredentialsException("Invalid email format");

            if (!CheckPassword(request.Password))
                throw new InvalidCredentialsException("Invalid password format");

            var existingUser = await userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidCredentialsException("User with this email already exists");

            var passwordHash = passwordHasher.HashPassword(request.Password);
            var user = new User(request.Email, RoleType.User);
            user.SetPasswordHash(passwordHash);

            await userRepository.AddAsync(user);
            await userRepository.SaveChangesAsync();

            var token = tokenGenerator.GenerateToken(user.Id, user.Email, user.Type);
            return new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Token = token
            };
        }

        private bool CheckEmail(string email)
        {
            var emailValidator = new EmailAddressAttribute();
            return emailValidator.IsValid(email);
        }
        private bool CheckPassword(string password)
        {
            var regex = new Regex(@"[a-zA-Z0-9\s]+");
            return password.Length >= 3 && regex.IsMatch(password);
        }
    }
}
