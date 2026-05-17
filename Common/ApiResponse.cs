namespace BaseApi.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public ApiResponse(bool success, string message, T? data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
            => new(true, message, data);

        public static ApiResponse<T> ErrorResponse(string message)
            => new(false, message, default);
    }
}

