namespace API.G3Operaciones.Garcias.MX.Application.Common.Dto
{
    public class ApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }

        public static ApiResponseDto Ok(string message, object? data = null)
            => new() { Success = true, Message = message, Data = data };

        public static ApiResponseDto Fail(string message)
            => new() { Success = false, Message = message };
    }
}
