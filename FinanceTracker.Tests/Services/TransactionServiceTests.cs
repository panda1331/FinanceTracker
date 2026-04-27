using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Application.Repository;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using FinanceTracker.Shared.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using Xunit;

namespace FinanceTracker.Tests.Services
{
    public class TransactionServiceTests
    {
        [Fact]
        public async Task CreateTransactionAsync_WithValidExpense_DecreasesBalance()
        {
            var userId = Guid.NewGuid();
            var account = new Account(AccountType.Cash, "name", userId);
            account.AddToBalance(500m);
            var category = new Category(userId, "cat", OperationType.Expense, false);
            var transaction = new Transaction(userId, account.Id, category.Id, OperationType.Expense, 100m, DateTime.UtcNow);

            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockBudgetService = new Mock<IBudgetService>();

            mockAccountRepository
                .Setup(r => r.GetByIdAsync(account.Id))
                .ReturnsAsync(account);
            mockCategoryRepository
                .Setup(r => r.GetByIdAsync(category.Id))
                .ReturnsAsync(category);

            mockAccountRepository
                .Setup(r => r.Update(account));
            mockTransactionRepository
                .Setup(r => r.AddAsync(It.IsAny<Transaction>()))
                .Returns(Task.CompletedTask);

            mockBudgetService
                .Setup(s => s.CheckBudgetExceedAsync(userId, category.Id, It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var service = new TransactionService(mockTransactionRepository.Object, mockAccountRepository.Object, mockCategoryRepository.Object, mockBudgetService.Object);
            var request = new CreateTransactionRequest
            {
                AccountId = account.Id,
                Date = DateTime.UtcNow,
                Amount = 100m,
                CategoryId = category.Id,
                Type = OperationType.Expense
            };
            var response = await service.CreateTransactionAsync(userId, request);
            account.Balance.Should().Be(400m);
            mockAccountRepository.Verify(r => r.Update(account), Times.Once);
            mockTransactionRepository.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
            mockTransactionRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateTransactionAsync_WithValidIncome_IncreasesBalance()
        {
            var userId = Guid.NewGuid();
            var account = new Account(AccountType.Cash, "name", userId);
            var category = new Category(userId, "cat", OperationType.Income, false);
            var transaction = new Transaction(userId, account.Id, category.Id, OperationType.Expense, 100m, DateTime.UtcNow);

            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockBudgetService = new Mock<IBudgetService>();

            mockAccountRepository
                .Setup(r => r.GetByIdAsync(account.Id))
                .ReturnsAsync(account);
            mockCategoryRepository
                .Setup(r => r.GetByIdAsync(category.Id))
                .ReturnsAsync(category);

            mockAccountRepository
                .Setup(r => r.Update(account));
            mockTransactionRepository
                .Setup(r => r.AddAsync(It.IsAny<Transaction>()))
                .Returns(Task.CompletedTask);

            mockBudgetService
                .Setup(s => s.CheckBudgetExceedAsync(userId, category.Id, It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            var service = new TransactionService(mockTransactionRepository.Object, mockAccountRepository.Object, mockCategoryRepository.Object, mockBudgetService.Object);
            var request = new CreateTransactionRequest
            {
                AccountId = account.Id,
                Date = DateTime.UtcNow,
                Amount = 100m,
                CategoryId = category.Id,
                Type = OperationType.Income
            };
            var response = await service.CreateTransactionAsync(userId, request);
            account.Balance.Should().Be(100m);
            mockAccountRepository.Verify(r => r.Update(account), Times.Once);
            mockTransactionRepository.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
            mockTransactionRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            mockBudgetService.Verify(s => s.CheckBudgetExceedAsync(userId, category.Id, It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CreateTransactionAsync_WithInsufficientFunds_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var account = new Account(AccountType.Cash, "name", userId);
            var category = new Category(userId, "cat", OperationType.Expense, false);

            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockBudgetService = new Mock<IBudgetService>();

            mockAccountRepository
                .Setup(r => r.GetByIdAsync(account.Id))
                .ReturnsAsync(account);
            mockCategoryRepository
                .Setup(r => r.GetByIdAsync(category.Id))
                .ReturnsAsync(category);

            mockAccountRepository
                .Setup(r => r.Update(account));

            var service = new TransactionService(mockTransactionRepository.Object, mockAccountRepository.Object, mockCategoryRepository.Object, mockBudgetService.Object);
            var request = new CreateTransactionRequest
            {
                AccountId = account.Id,
                Date = DateTime.UtcNow,
                Amount = 200m,
                CategoryId = category.Id,
                Type = OperationType.Expense
            };

            Func<Task> act = async () => await service.CreateTransactionAsync(userId, request);
            await act.Should()
                .ThrowAsync<ValidationException>()
                .WithMessage("Insufficient funds");
        }

        [Fact]
        public async Task CreateTransactionAsync_WithMismatchedTypes_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var account = new Account(AccountType.Cash, "name", userId);
            var category = new Category(userId, "cat", OperationType.Expense, false);

            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockBudgetService = new Mock<IBudgetService>();

            mockAccountRepository
                .Setup(r => r.GetByIdAsync(account.Id))
                .ReturnsAsync(account);
            mockCategoryRepository
                .Setup(r => r.GetByIdAsync(category.Id))
                .ReturnsAsync(category);

            mockAccountRepository
                .Setup(r => r.Update(account));

            var service = new TransactionService(mockTransactionRepository.Object, mockAccountRepository.Object, mockCategoryRepository.Object, mockBudgetService.Object);
            var request = new CreateTransactionRequest
            {
                AccountId = account.Id,
                Date = DateTime.UtcNow,
                Amount = 200m,
                CategoryId = category.Id,
                Type = OperationType.Income
            };

            Func<Task> act = async () => await service.CreateTransactionAsync(userId, request);
            await act.Should()
                .ThrowAsync<ValidationException>()
                .WithMessage("Category and transaction types do not match");
        }

        [Fact]
        public async Task CreateTransactionAsync_WithNonExistentAccount_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var category = new Category(userId, "cat", OperationType.Expense, false);

            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockBudgetService = new Mock<IBudgetService>();

            mockAccountRepository
                .Setup(r => r.GetByIdAsync(accountId))
                .ReturnsAsync((Account?)null);
            mockCategoryRepository
                .Setup(r => r.GetByIdAsync(category.Id))
                .ReturnsAsync(category);

            var service = new TransactionService(mockTransactionRepository.Object, mockAccountRepository.Object, mockCategoryRepository.Object, mockBudgetService.Object);
            var request = new CreateTransactionRequest
            {
                AccountId = accountId,
                Date = DateTime.UtcNow,
                Amount = 200m,
                CategoryId = category.Id,
                Type = OperationType.Expense
            };

            Func<Task> act = async () => await service.CreateTransactionAsync(userId, request);
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("No such account");
        }

        [Fact]
        public async Task CreateTransactionAsync_WithNonExistentCategory_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var account = new Account(AccountType.Cash, "name", userId);
            var categoryId = Guid.NewGuid();

            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockBudgetService = new Mock<IBudgetService>();

            mockAccountRepository
                .Setup(r => r.GetByIdAsync(account.Id))
                .ReturnsAsync(account);
            mockCategoryRepository
                .Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync((Category?)null);

            var service = new TransactionService(mockTransactionRepository.Object, mockAccountRepository.Object, mockCategoryRepository.Object, mockBudgetService.Object);
            var request = new CreateTransactionRequest
            {
                AccountId = account.Id,
                Date = DateTime.UtcNow,
                Amount = 200m,
                CategoryId = categoryId,
                Type = OperationType.Expense
            };

            Func<Task> act = async () => await service.CreateTransactionAsync(userId, request);
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("No such category");
        }

        [Fact]
        public async Task GetTransactionsByUserIdAsync_ReturnsList()
        {
            var userId = Guid.NewGuid();
            var transactions = new List<Transaction>
            {
                new Transaction(userId, Guid.NewGuid(), Guid.NewGuid(), OperationType.Expense, 100m, DateTime.UtcNow),
                new Transaction(userId, Guid.NewGuid(), Guid.NewGuid(), OperationType.Expense, 200m, DateTime.UtcNow),
            };
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            mockTransactionRepository
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(transactions);

            var mockAccountRepository = new Mock<IAccountRepository>();
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            var mockBudgetService = new Mock<IBudgetService>();

            var service = new TransactionService(mockTransactionRepository.Object, mockAccountRepository.Object, mockCategoryRepository.Object, mockBudgetService.Object);
            var response = await service.GetTransactionsByUserIdAsync(userId);

            response.Should().NotBeNull();
            response.Should().HaveCount(2);
        }

        [Fact]
        public async Task DeleteTransactionAsync_WithValidId_DeletesTransactionAndRestoresBalance()
        {
            var userId = Guid.NewGuid();
            var account = new Account(AccountType.Cash, "name", userId);
            var category = new Category(userId, "cat", OperationType.Income, false);
            var transaction = new Transaction(userId, account.Id, category.Id, OperationType.Expense, 100m, DateTime.UtcNow);

            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository
                .Setup(r => r.GetByIdAsync(account.Id))
                .ReturnsAsync(account);

            var mockTransactionRepository = new Mock<ITransactionRepository>();
            mockTransactionRepository
                .Setup(r => r.GetByIdAsync(transaction.Id))
                .ReturnsAsync(transaction);
            
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            var mockBudgetService = new Mock<IBudgetService>();

            var service = new TransactionService(mockTransactionRepository.Object, mockAccountRepository.Object, mockCategoryRepository.Object, mockBudgetService.Object);
            await service.DeleteTransactionAsync(userId, transaction.Id);

            mockAccountRepository.Verify(r => r.Update(account));
            mockTransactionRepository.Verify(r => r.Delete(transaction));
            mockTransactionRepository.Verify(r => r.SaveChangesAsync());
            account.Balance.Should().Be(100m);
        }
    }
}
