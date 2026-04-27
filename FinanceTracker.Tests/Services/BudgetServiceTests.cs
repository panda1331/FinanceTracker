using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.Repository;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using FinanceTracker.Shared.Exceptions;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Xunit;
using ValidationException = FinanceTracker.Shared.Exceptions.ValidationException;

namespace FinanceTracker.Tests.Services
{
    public class BudgetServiceTests
    {
        [Fact]
        public async Task CreateOrUpdateBudgetAsync_CreatesNewBudget_ReturnsBudgetResponse()
        {
            var userId = Guid.NewGuid();
            var category = new Category(userId, "cat", OperationType.Expense, false);

            var mockBudgetRepository = new Mock<IBudgetRepository>();
            mockBudgetRepository
                .Setup(r => r.GetByCategoryAndMonthAsync(userId, category.Id, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((Budget?)null);

            var mockTransactionRepository = new Mock<ITransactionRepository>();

            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(r => r.GetByIdAsync(category.Id))
                .ReturnsAsync(category);

            var request = new CreateBudgetRequest
            {
                CategoryId = category.Id,
                Limit = 500m,
                Month = 1,
                Year = 2026
            };
            var service = new BudgetService(mockBudgetRepository.Object, mockTransactionRepository.Object, mockCategoryRepository.Object);
            var response = await service.CreateOrUpdateBudgetAsync(userId, request);

            response.Should().NotBeNull();
            response.Limit.Should().Be(500m);
            response.Month.Should().Be(1);
            response.Year.Should().Be(2026);

            mockBudgetRepository.Verify(r => r.AddAsync(It.IsAny<Budget>()), Times.Once);
            mockBudgetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateOrUpdateBudgetAsync_UpdatesExistingBudget_ReturnsBudgetResponse()
        {
            var userId = Guid.NewGuid();
            var category = new Category(userId, "cat", OperationType.Expense, false);
            var budget = new Budget(userId, category.Id, 500m, 4, 2026);

            var mockBudgetRepository = new Mock<IBudgetRepository>();
            mockBudgetRepository
                .Setup(r => r.GetByCategoryAndMonthAsync(userId, category.Id, 4, 2026))
                .ReturnsAsync(budget);

            var mockTransactionRepository = new Mock<ITransactionRepository>();

            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(r => r.GetByIdAsync(category.Id))
                .ReturnsAsync(category);

            var request = new CreateBudgetRequest
            {
                CategoryId = category.Id,
                Limit = 500m,
                Month = 4,
                Year = 2026
            };
            var service = new BudgetService(mockBudgetRepository.Object, mockTransactionRepository.Object, mockCategoryRepository.Object);
            var response = await service.CreateOrUpdateBudgetAsync(userId, request);

            response.Should().NotBeNull();
            response.Limit.Should().Be(500m);
            response.Month.Should().Be(4);
            response.Year.Should().Be(2026);

            mockBudgetRepository.Verify(r => r.Update(It.IsAny<Budget>()), Times.Once);
            mockBudgetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateOrUpdateBudgetAsync_WithIncomeCategory_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var category = new Category(userId, "cat", OperationType.Income, false);

            var mockBudgetRepository = new Mock<IBudgetRepository>();

            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(r => r.GetByIdAsync(category.Id))
                .ReturnsAsync(category);

            var mockTransactionRepository = new Mock<ITransactionRepository>();

            var request = new CreateBudgetRequest
            {
                CategoryId = category.Id,
                Limit = 500m,
                Month = 4,
                Year = 2026
            };
            var service = new BudgetService(mockBudgetRepository.Object, mockTransactionRepository.Object, mockCategoryRepository.Object);
            Func<Task> act = async () => await service.CreateOrUpdateBudgetAsync(userId, request);
            await act.Should()
                .ThrowAsync<ValidationException>()
                .WithMessage("Limits can not be on put on income categoreis");
        }

        [Fact]
        public async Task GetBudgetsByUserIdAsync_ReturnsListWithIsExceeded()
        {
            var userId = Guid.NewGuid();
            var budgets = new List<Budget>
            {
                new Budget(userId, Guid.NewGuid(), 200m, 3, 2026),
                new Budget(userId, Guid.NewGuid(), 300m, 4, 2026)
            };
            var transactions = new List<Transaction>
            {
                new Transaction(userId, Guid.NewGuid(), budgets[0].CategoryId, OperationType.Expense, 250m, DateTime.UtcNow)
            };

            var mockBudgetRepository = new Mock<IBudgetRepository>();
            mockBudgetRepository
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(budgets);

            var mockTransactionRepository = new Mock<ITransactionRepository>();
            mockTransactionRepository
                .Setup(r => r.GetByCategoryAndPeriodAsync(userId, budgets[0].CategoryId, 3, 2026))
                .ReturnsAsync(transactions); 

            mockTransactionRepository
                .Setup(r => r.GetByCategoryAndPeriodAsync(userId, budgets[1].CategoryId, 4, 2026))
                .ReturnsAsync(new List<Transaction>());

            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                 .Setup(r => r.GetByIdAsync(budgets[0].CategoryId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new Category(userId, "cat1", OperationType.Expense, false));

            mockCategoryRepository
                .Setup(r => r.GetByIdAsync(budgets[1].CategoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Category(userId, "cat2", OperationType.Expense, false));

            var service = new BudgetService(mockBudgetRepository.Object, mockTransactionRepository.Object, mockCategoryRepository.Object);
            var response = await service.GetBudgetsByUserIdAsync(userId);

            response.Should().NotBeNull();
            response.Should().HaveCount(2);
            response[0].IsExceeded.Should().BeTrue();
            response[1].IsExceeded.Should().BeFalse();
        }

        [Fact]
        public async Task CheckBudgetExceedAsync_WhenExceeded_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var month = 4;
            var year = 2026;

            var budget = new Budget(userId, categoryId, 200m, month, year);
            var transactions = new List<Transaction>
            {
                new Transaction(userId, Guid.NewGuid(), budget.CategoryId, OperationType.Expense, 250m, DateTime.UtcNow)
            };

            var mockBudgetRepository = new Mock<IBudgetRepository>();
            mockBudgetRepository
                .Setup(r => r.GetByCategoryAndMonthAsync(userId, categoryId, month, year))
                .ReturnsAsync(budget);

            var mockTransactionRepository = new Mock<ITransactionRepository>();
            mockTransactionRepository
                .Setup(r => r.GetByCategoryAndPeriodAsync(userId, categoryId, month, year))
                .ReturnsAsync(transactions);

            var mockCategoryRepository = new Mock<ICategoryRepository>();

            var service = new BudgetService(mockBudgetRepository.Object, mockTransactionRepository.Object, mockCategoryRepository.Object);
            Func<Task> act = async () => await service.CheckBudgetExceedAsync(userId, categoryId, month, year);
            await act.Should()
                .ThrowAsync<BudgetExceededException>()
                .WithMessage("Budget is exceeded");
        }

        [Fact]
        public async Task CheckBudgetExceedAsync_WhenNoBudget_DoesNotThrow()
        {
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var month = 4;
            var year = 2026;

            var mockBudgetRepository = new Mock<IBudgetRepository>();
            mockBudgetRepository
                .Setup(r => r.GetByCategoryAndMonthAsync(userId, categoryId, month, year))
                .ReturnsAsync((Budget?)null);

            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockCategoryRepository = new Mock<ICategoryRepository>();

            var service = new BudgetService(mockBudgetRepository.Object, mockTransactionRepository.Object, mockCategoryRepository.Object);
            Func<Task> act = async () => await service.CheckBudgetExceedAsync(userId, categoryId, month, year);
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteBudgetAsync_WithValidId_DeletesBudget()
        {
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var month = 4;
            var year = 2026;

            var budget = new Budget(userId, categoryId, 200m, month, year);

            var mockBudgetRepository = new Mock<IBudgetRepository>();
            mockBudgetRepository
                .Setup(r => r.GetByIdAsync(budget.Id))
                .ReturnsAsync(budget);

            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockCategoryRepository = new Mock<ICategoryRepository>();

            var service = new BudgetService(mockBudgetRepository.Object, mockTransactionRepository.Object, mockCategoryRepository.Object);
            await service.DeleteBudgetAsync(userId, budget.Id);
            
            mockBudgetRepository.Verify(r => r.Delete(budget), Times.Once);
            mockBudgetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteBudgetAsync_WithWrongUser_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var month = 4;
            var year = 2026;

            var budget = new Budget(userId, categoryId, 200m, month, year);

            var mockBudgetRepository = new Mock<IBudgetRepository>();
            mockBudgetRepository
                .Setup(r => r.GetByIdAsync(budget.Id))
                .ReturnsAsync(budget);

            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockCategoryRepository = new Mock<ICategoryRepository>();

            var service = new BudgetService(mockBudgetRepository.Object, mockTransactionRepository.Object, mockCategoryRepository.Object);
            Func<Task> act = async () => await service.DeleteBudgetAsync(otherUserId, budget.Id);
            await act.Should().ThrowAsync<ValidationException>().WithMessage("You can only delete your own budgets");
        }
    }
}
