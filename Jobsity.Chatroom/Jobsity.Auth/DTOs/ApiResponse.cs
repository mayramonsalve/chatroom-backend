namespace Jobsity.Auth.DTOs
{
    public class ApiResponse<T>
    {
        public ApiResponse(T data)
        {
            Data = data;
        }
        public ApiResponse(T data, string message)
        {
            Data = data;
            Message = message;
        }
        public T Data { get; set; }
        public string Message { get; set; }
    }
}
