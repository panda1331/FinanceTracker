using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.Repository;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using FinanceTracker.Shared.Exceptions;
using FluentAssertions;
using FluentAssertions.Equivalency.Steps;
using Moq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace FinanceTracker.Tests.Services
{
    public class CategoryServiceTests
    {
        [Fact]
        public async Task CreateCategoryAsync_WithValidData_ReturnsCategoryResponse()
        {
            var userId = Guid.NewGuid();
            var name = "name";
            var type = OperationType.Expense;

            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(r => r.AddAsync(It.IsAny<Category>()))
                .Returns(Task.CompletedTask);
            mockCategoryRepository
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var service = new CategoryService(mockCategoryRepository.Object);
            var request = new CreateCategoryRequest { Name = name, Type = type };
            var response = await service.CreateCategoryAsync(userId, request);

            response.Should().NotBeNull();
            response.Name.Should().Be(name);
            response.Type.Should().Be(type);
            response.IsDefault.Should().BeFalse();
        }

        [Fact]
        public async Task CreateCategoryAsync_WithRepositoryError_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var name = "name";
            var type = OperationType.Expense;

            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(r => r.AddAsync(It.IsAny<Category>()))
                .ThrowsAsync(new Exception("Database error"));

            var service = new CategoryService(mockCategoryRepository.Object);
            var request = new CreateCategoryRequest { Name = name, Type = type };

            Func<Task> act = async () => await service.CreateCategoryAsync(userId, request);
            await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
        }

        [Fact]
        public async Task GetCategoriesByUserIdAsync_ReturnsUserAndDefaultCategories()
        {
            var userId = Guid.NewGuid();
            var categories = new List<Category>
            {
                new Category(userId, "cat1", OperationType.Expense, false),
                new Category(userId, "cat2", OperationType.Income, false)
            };

            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(categories);

            var service = new CategoryService(mockCategoryRepository.Object);
            var response = await service.GetCategoriesByUserIdAsync(userId);

            response.Should().NotBeNull();
            response.Should().HaveCount(2);
            response[0].Name.Should().Be("cat1");
            response[1].Name.Should().Be("cat2");
        }

        [Fact]
        public async Task GetCategoriesByUserIdAsync_ReturnsOnlyDefault_WhenNoUserCategories()
        {
            var userId = Guid.NewGuid();
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<Category>());

            var service = new CategoryService(mockCategoryRepository.Object);
            var response = await service.GetCategoriesByUserIdAsync(userId);

            response.Should().NotBeNull();
            response.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteCategoryAsync_WithValidId_DeletesCategory()
        {
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var name = "name";
            var type = OperationType.Expense;

            var category = new Category(userId, name, type, false);
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync(category);

            var service = new CategoryService(mockCategoryRepository.Object);
            await service.DeleteCategoryAsync(userId, categoryId);

            mockCategoryRepository.Verify(r => r.Delete(category));
            mockCategoryRepository.Verify(r => r.SaveChangesAsync());
        }

        [Fact]
        public async Task DeleteCategoryAsync_WhenDefaultCategory_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var category = new Category(null, "Food", OperationType.Expense, true);

            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync(category);

            var service = new CategoryService(mockCategoryRepository.Object);
            Func<Task> act = async () => await service.DeleteCategoryAsync(userId, categoryId);
            await act.Should().ThrowAsync<ValidationException>().WithMessage("Cannot delete default category");
        }
    }
}
