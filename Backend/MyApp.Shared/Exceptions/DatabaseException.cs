using MyApp.Shared.DTOs;

namespace MyApp.Shared.Exceptions
{
    public class DatabaseException : CustomException
    {
        public DatabaseException(string message, Exception? innerException = null)
            : base(message, 500, innerException?.Message)
        {
        }
    }
}
