using FinanceTracker.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Application.Interfaces.Services
{
    public interface IAnalyticsService
    {
        Task<AnalyticsResponse> GetAnalyticsAsync(Guid userId, int month, int year);
    }
}
