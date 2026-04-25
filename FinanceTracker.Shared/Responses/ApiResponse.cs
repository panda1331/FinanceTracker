using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Shared.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; }

        public static ApiResponse<T> SuccessResponse(T data)
        {
            var response = new ApiResponse<T>();    
            response.Success = true;
            response.Data = data;
            return response;
        }
        public static ApiResponse<T> Error(string message)
        {
            var response = new ApiResponse<T>();
            response.Success = false;
            response.Message = message;
            return response;
        }
    }
}
