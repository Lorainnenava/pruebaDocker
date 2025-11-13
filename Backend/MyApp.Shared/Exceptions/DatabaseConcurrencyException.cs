using MyApp.Shared.DTOs;

namespace MyApp.Shared.Exceptions
{
    public class DatabaseConcurrencyException : CustomException
    {
        public DatabaseConcurrencyException(string message, Exception? innerException = null)
            : base(message, 409, innerException?.Message)
        {
        }
    }
}
