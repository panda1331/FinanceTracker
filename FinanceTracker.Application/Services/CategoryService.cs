using FinanceTracker.Application.DTOs.Requests;
using FinanceTracker.Application.DTOs.Responses;
using FinanceTracker.Application.Interfaces.Services;
using FinanceTracker.Application.Repository;
using FinanceTracker.Domain.Models;
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

        public async Task<List<CategoryResponse>> GetCategoriesByUserIdAsync(Guid userId)
        {
            var categories = await _repository.GetByUserIdAsync(userId);
            return MapCategoriesToResponses(categories);
        }

        private List<CategoryResponse> MapCategoriesToResponses(List<Category> categories)
        {
            var responses = new List<CategoryResponse>();
            foreach(var category in categories)
            {
                var response = new CategoryResponse()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Type = category.Type,
                    IsDefault = category.IsDefault,
                };
                responses.Add(response);
            }
            return responses;
        }
    }
}
