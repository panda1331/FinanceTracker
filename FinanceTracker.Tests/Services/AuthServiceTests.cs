using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.Interfaces.Infrastructure;
using FinanceTracker.Application.Repository;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Models;
using FinanceTracker.Shared.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FinanceTracker.Tests.Services
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task RegisterAsync_WithValidData_ReturnsAuthResponse()
        {
            var email = "test@gmail.com";
            var password = "1234";

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync((User?)null);

            var mockPasswordHasher = new Mock<IPasswordHasher>();
            mockPasswordHasher
                .Setup(h => h.HashPassword(password))
                .Returns("hashed");

            var mockTokenGenerator = new Mock<ITokenGenerator>();
            mockTokenGenerator
                .Setup(g => g.GenerateToken(It.IsAny<Guid>(), email, Domain.Enums.RoleType.User))
                .Returns("token");

            var regitryRequest = new RegisterRequest{ Email = email, Password = password };

            var authService = new AuthService(mockUserRepository.Object, mockPasswordHasher.Object, mockTokenGenerator.Object);
            var response = await authService.RegisterAsync(regitryRequest);

            response.Should().NotBeNull();
            response.Token.Should().NotBeNull();
            response.Email.Should().Be(email);
            mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once());
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ThrowsException()
        {
            var email = "test@gmail.com";
            var password = "1234";

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(new User(email, Domain.Enums.RoleType.User));

            var mockTokenGenerator = new Mock<ITokenGenerator>();
            var mockPasswordHasher = new Mock<IPasswordHasher>();
            var registryRequest = new RegisterRequest { Email = email, Password =password };

            var authService = new AuthService(mockUserRepository.Object, mockPasswordHasher.Object, mockTokenGenerator.Object);
            Func<Task> act = async () => await authService.RegisterAsync(registryRequest);

            await act.Should().ThrowAsync<InvalidCredentialsException>().WithMessage("User with this email already exists");
        }

        [Fact]
        public async Task RegisterAsync_WithInvalidEmail_ThrowsException()
        {
            var email = "test.com";
            var password = "1234";

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenGenerator = new Mock<ITokenGenerator>();
            var mockPasswordHasher = new Mock<IPasswordHasher>();
            var registryRequest = new RegisterRequest { Email = email, Password = password };

            var authService = new AuthService(mockUserRepository.Object, mockPasswordHasher.Object, mockTokenGenerator.Object);
            Func<Task> act = async () => await authService.RegisterAsync(registryRequest);

            await act.Should().ThrowAsync<InvalidCredentialsException>().WithMessage("Invalid email format");
        }

        [Fact]
        public async Task RegisterAsync_WithInvalidPassword_ThrowsException()
        {
            var email = "test@gmail.com";
            var password = "ee";

            var mockUserRepository = new Mock<IUserRepository>();
            var mockTokenGenerator = new Mock<ITokenGenerator>();
            var mockPasswordHasher = new Mock<IPasswordHasher>();
            var registryRequest = new RegisterRequest { Email = email, Password = password };

            var service = new AuthService(mockUserRepository.Object, mockPasswordHasher.Object, mockTokenGenerator.Object);
            Func<Task> act = async () => await service.RegisterAsync(registryRequest);

            await act.Should().ThrowAsync<InvalidCredentialsException>().WithMessage("Invalid password format");
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
        {
            var email = "tesr@gmail.com";
            var password = "1234";

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(new User(email, Domain.Enums.RoleType.User));

            var mockPasswordHasher = new Mock<IPasswordHasher>();
            mockPasswordHasher
                .Setup(h => h.VerifyPassword(password, It.IsAny<string>()))
                .Returns(true);

            var mockTokenGenerator = new Mock<ITokenGenerator>();
            mockTokenGenerator
                .Setup(g => g.GenerateToken(It.IsAny<Guid>(), email, Domain.Enums.RoleType.User))
                .Returns("token");

            var service = new AuthService(mockUserRepository.Object, mockPasswordHasher.Object, mockTokenGenerator.Object);
            var response = await service.LoginAsync(new LoginRequest { Email = email, Password = password });

            response.Should().NotBeNull();
            response.Email.Should().Be(email);
            response.Token.Should().NotBeNull();
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ThrowsException()
        {
            var email = "test@gmail.com";
            var password = "password";

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(new User(email, Domain.Enums.RoleType.User));

            var mockPasswordHasher = new Mock<IPasswordHasher>();
            mockPasswordHasher
                .Setup(h => h.VerifyPassword(password, It.IsAny<string>()))
                .Returns(false);

            var mockTokenGenerator = new Mock<ITokenGenerator>();

            var service = new AuthService(mockUserRepository.Object, mockPasswordHasher.Object, mockTokenGenerator.Object);
            Func<Task> act = async () => await service.LoginAsync(new LoginRequest { Email = email, Password = password });
            await act.Should().ThrowAsync<InvalidCredentialsException>().WithMessage("Invalid email or password");
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentEmail_ThrowsException()
        {
            var email = "eee@gmail.com";
            var mockUserRepositiry = new Mock<IUserRepository>();
            mockUserRepositiry.Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync((User?)null);

            var mockTokenGenerator = new Mock<ITokenGenerator>();
            var mockPasswordHasher = new Mock<IPasswordHasher>();

            var service = new AuthService(mockUserRepositiry.Object, mockPasswordHasher.Object, mockTokenGenerator.Object);
            Func<Task> act = async () => await service.LoginAsync(new LoginRequest { Email = email, Password = "password" });
            await act.Should().ThrowAsync<InvalidCredentialsException>();
        }
    }
}
