using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Application.Repository;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Enums;
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
    public class AccountServiceTests
    {
        [Fact]
        public async Task CreateAccountAsync_WithValidData_ReturnsAccountResponse()
        {
            var id = Guid.NewGuid();
            var type = AccountType.CreditCard;
            var name = "name";
            
            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository
                .Setup(r => r.AddAsync(It.IsAny<Account>()))
                .Returns(Task.CompletedTask);
            mockAccountRepository
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            var accountService = new AccountService(mockAccountRepository.Object);
            var request = new CreateAccountRequest { Name = name, Type = type };
            var response = await accountService.CreateAccountAsync(id, request);

            response.Should().NotBeNull();
            response.Name.Should().Be(name);
            response.Type.Should().Be(type);
            response.Balance.Should().Be(0m);
        }

        [Fact]
        public async Task CreateAccountAsync_WithRepositoryError_ThrowsException()
        {
            var id = Guid.NewGuid();
            var type = AccountType.CreditCard;
            var name = "name";

            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository
                .Setup(r => r.AddAsync(It.IsAny<Account>()))
                .ThrowsAsync(new Exception("Database error"));
           
            var accountService = new AccountService(mockAccountRepository.Object);
            var request = new CreateAccountRequest { Name = name, Type = type };

            Func<Task> act = async () => await accountService.CreateAccountAsync(id, request);
            await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
        }

        [Fact]
        public async Task GetAccountsByUserIdAsync_WithExistingAccounts_ReturnsList()
        {
            var userId = Guid.NewGuid();
            var accounts = new List<Account>
            {
                new Account(AccountType.Cash, "cash", userId),
                new Account(AccountType.CreditCard, "card", userId)
            };

            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(accounts);

            var service = new AccountService(mockAccountRepository.Object);
            var response = await service.GetAccountsByUserIdAsync(userId);

            response.Should().NotBeNull();
            response.Should().HaveCount(2);
            response[0].Name.Should().Be("cash");
            response[1].Name.Should().Be("card");
        }

        [Fact]
        public async Task GetAccountsByUserIdAsync_WithNoAccounts_ReturnsEmptyList()
        {
            var userId = Guid.NewGuid();

            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<Account>());

            var service = new AccountService(mockAccountRepository.Object);
            var response = await service.GetAccountsByUserIdAsync(userId);

            response.Should().NotBeNull();
            response.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteAccountAsync_WithValidId_DeletesAccount()
        {
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();

            var account = new Account(AccountType.Cash, "name", userId);
            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository
                .Setup(r => r.GetByIdAsync(accountId))
                .ReturnsAsync(account);

            var service = new AccountService(mockAccountRepository.Object);
            await service.DeleteAccountAsync(userId, accountId);

            mockAccountRepository.Verify(r => r.Delete(account), Times.Once);
            mockAccountRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAccountAsync_WithNonExistentId_ThrowsNotFoundException()
        {
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();

            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository
                .Setup(r => r.GetByIdAsync(accountId))
                .ReturnsAsync((Account?) null);

            var service = new AccountService(mockAccountRepository.Object);
            Func<Task> act = async () => await service.DeleteAccountAsync(userId, accountId);
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("No such account");
        }
    }
}
