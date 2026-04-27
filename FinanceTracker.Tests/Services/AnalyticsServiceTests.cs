using FinanceTracker.Application.Repository;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FinanceTracker.Tests.Services
{
    public class AnalyticsServiceTests
    {
        [Fact]
        public async Task GetAnalyticsAsync_WithExpenses_ReturnsCorrectTotalExpense()
        {
            var userId = Guid.NewGuid();
            var month = 4;
            var year = 2026;

           
            var categories = new List<Category>
            {
                new Category(userId, "Food", OperationType.Expense, false)
            };
            var transactions = new List<Transaction>
            {
                new Transaction(userId, Guid.NewGuid(), categories[0].Id, OperationType.Expense, 250m, DateTime.UtcNow)
            };

            var mockTransactionRepository = new Mock<ITransactionRepository>();
            mockTransactionRepository
                .Setup(r => r.GetByUserIdAndPeriodAsync(userId, month, year))
                .ReturnsAsync(transactions);

            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(categories);

            var mockBudgetRepository = new Mock<IBudgetRepository>();
            mockBudgetRepository
                .Setup(r => r.GetByCategoryAndMonthAsync(userId, categories[0].Id, month, year))
                .ReturnsAsync((Budget?)null);

            var service = new AnalyticsService(mockTransactionRepository.Object, mockCategoryRepository.Object, mockBudgetRepository.Object);
            var response = await service.GetAnalyticsAsync(userId, month, year);

            response.Should().NotBeNull();
            response.TotalExpense.Should().Be(250m);
            response.Items.Should().HaveCount(1);
            response.Items[0].CategoryName.Should().Be("Food");
            response.Items[0].TotalAmount.Should().Be(250m);
            response.Items[0].IsExceeded.Should().BeFalse();
        }

        [Fact]
        public async Task GetAnalyticsAsync_WithExpenses_GroupsByCategory()
        {
            var userId = Guid.NewGuid();
            var month = 4;
            var year = 2026;


            var categories = new List<Category>
            {
                new Category(userId, "Food", OperationType.Expense, false),
                new Category(userId, "Transport", OperationType.Expense, false)
            };
            var transactions = new List<Transaction>
            {
                new Transaction(userId, Guid.NewGuid(), categories[0].Id, OperationType.Expense, 100m, DateTime.UtcNow),
                new Transaction(userId, Guid.NewGuid(), categories[0].Id, OperationType.Expense, 50m, DateTime.UtcNow),
                new Transaction(userId, Guid.NewGuid(), categories[1].Id, OperationType.Expense, 200m, DateTime.UtcNow),
            };
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            mockTransactionRepository
                .Setup(r => r.GetByUserIdAndPeriodAsync(userId, month, year, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);

            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            var mockBudgetRepository = new Mock<IBudgetRepository>();
            mockBudgetRepository
                .Setup(r => r.GetByCategoryAndMonthAsync(userId, It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Budget?)null);

            var service = new AnalyticsService(mockTransactionRepository.Object, mockCategoryRepository.Object, mockBudgetRepository.Object);
            var response = await service.GetAnalyticsAsync(userId, month, year);

            categories[0].Id.Should().NotBe(categories[1].Id);
            transactions[0].CategoryId.Should().Be(categories[0].Id);
            transactions[2].CategoryId.Should().Be(categories[1].Id);

            response.Should().NotBeNull();
            response.TotalExpense.Should().Be(350m);
            response.Items.Should().HaveCount(2);
            response.Items.Should().Contain(i => i.CategoryName == "Food" && i.TotalAmount == 150m);
            response.Items.Should().Contain(i => i.CategoryName == "Transport" && i.TotalAmount == 200m);
        }

        [Fact]
        public async Task GetAnalyticsAsync_WhenOverBudget_SetsIsExceededTrue()
        {
            var userId = Guid.NewGuid();
            var month = 4;
            var year = 2026;


            var categories = new List<Category>
            {
                new Category(userId, "Food", OperationType.Expense, false)
            };
            var transactions = new List<Transaction>
            {
                new Transaction(userId, Guid.NewGuid(), categories[0].Id, OperationType.Expense, 250m, DateTime.UtcNow)
            };
            var budget = new Budget(userId, categories[0].Id, 200m, month, year);

            var mockTransactionRepository = new Mock<ITransactionRepository>();
            mockTransactionRepository
                .Setup(r => r.GetByUserIdAndPeriodAsync(userId, month, year))
                .ReturnsAsync(transactions);

            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(categories);

            var mockBudgetRepository = new Mock<IBudgetRepository>();
            mockBudgetRepository
                .Setup(r => r.GetByCategoryAndMonthAsync(userId, categories[0].Id, month, year))
                .ReturnsAsync(budget);

            var service = new AnalyticsService(mockTransactionRepository.Object, mockCategoryRepository.Object, mockBudgetRepository.Object);
            var response = await service.GetAnalyticsAsync(userId, month, year);

            response.Items[0].IsExceeded.Should().BeTrue();
            response.Items[0].Limit.Should().Be(200m);
        }

        [Fact]
        public async Task GetAnalyticsAsync_WhenWithinBudget_SetsIsExceededFalse()
        {
            var userId = Guid.NewGuid();
            var month = 4;
            var year = 2026;


            var categories = new List<Category>
            {
                new Category(userId, "Food", OperationType.Expense, false)
            };
            var transactions = new List<Transaction>
            {
                new Transaction(userId, Guid.NewGuid(), categories[0].Id, OperationType.Expense, 150m, DateTime.UtcNow)
            };
            var budget = new Budget(userId, categories[0].Id, 200m, month, year);

            var mockTransactionRepository = new Mock<ITransactionRepository>();
            mockTransactionRepository
                .Setup(r => r.GetByUserIdAndPeriodAsync(userId, month, year))
                .ReturnsAsync(transactions);

            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(categories);

            var mockBudgetRepository = new Mock<IBudgetRepository>();
            mockBudgetRepository
                .Setup(r => r.GetByCategoryAndMonthAsync(userId, categories[0].Id, month, year))
                .ReturnsAsync(budget);

            var service = new AnalyticsService(mockTransactionRepository.Object, mockCategoryRepository.Object, mockBudgetRepository.Object);
            var response = await service.GetAnalyticsAsync(userId, month, year);

            response.Items[0].IsExceeded.Should().BeFalse();
            response.Items[0].Limit.Should().Be(200m);
        }

        [Fact]
        public async Task GetAnalyticsAsync_WithNoExpenses_ReturnsEmptyItems()
        {
            var userId = Guid.NewGuid();
            var month = 4;
            var year = 2026;

            var mockTransactionRepository = new Mock<ITransactionRepository>();
            mockTransactionRepository
                .Setup(r => r.GetByUserIdAndPeriodAsync(userId, month, year))
                .ReturnsAsync(new List<Transaction>());

            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<Category>());

            var mockBudgetRepository = new Mock<IBudgetRepository>();

            var service = new AnalyticsService(mockTransactionRepository.Object, mockCategoryRepository.Object, mockBudgetRepository.Object);
            var response = await service.GetAnalyticsAsync(userId, month, year);

            response.Should().NotBeNull();
            response.TotalExpense.Should().Be(0m);
            response.Items.Should().BeEmpty();
        }
    }
}
