using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Application.Repository;
using FinanceTracker.Domain.Models;
using FinanceTracker.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<CategoryResponse> CreateCategoryAsync(Guid userId, CreateCategoryRequest request)
        {
            var category = new Category(userId, request.Name, request.Type, false);
            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();
            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type,
                IsDefault = category.IsDefault,
            };
        }

        public async Task DeleteCategoryAsync(Guid userId, Guid categoryId)
        {
            var category = await _repository.GetByIdAsync(categoryId);
            if (category == null)
                throw new NotFoundException("No such category");

            if (category.IsDefault)
                throw new ValidationException("Cannot delete default category");

            if (category.UserId != userId)
                throw new ValidationException("You can only delete wour own categories");

            _repository.Delete(category);
            await _repository.SaveChangesAsync();
        }

        public async Task<List<CategoryResponse>> GetCategoriesByUserIdAsync(Guid userId)
        {
            var categories = await _repository.GetByUserIdAsync(userId);
            return Mapper.ToResponses(categories);
        }

        public async Task<CategoryResponse> UpdateCategoryAsync(Guid userId, Guid categoryId, UpdateCategoryRequest request)
        {
            var category = await _repository.GetByIdAsync(categoryId);
            if (category == null)
                throw new NotFoundException("No such category");

            if (category.IsDefault)
                throw new ValidationException("Cannot update default category");

            if (category.UserId != userId)
                throw new ValidationException("You can only update your own categories");

            category.UpdateName(request.Name);
            _repository.Update(category);
            await _repository.SaveChangesAsync();

            return new CategoryResponse
            {
                Id = category.Id,
                IsDefault = category.IsDefault,
                Name = category.Name,
                Type = category.Type,
            };
        }
    }
}
