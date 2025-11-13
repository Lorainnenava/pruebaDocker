namespace MyApp.Shared.DTOs
{
    public class CustomException : Exception
    {
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Details { get; set; }

        protected CustomException(string errorMessage,int statusCode= 500, string? details = null)
            : base(errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
            Details = details;
        }
    }
}
