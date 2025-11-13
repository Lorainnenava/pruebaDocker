using MyApp.Shared.DTOs;

namespace MyApp.Shared.Exceptions
{
    public class GeneralException : CustomException
    {
        public GeneralException(string message, Exception? innerException = null)
            : base(message, 500, innerException?.Message)
        {
        }
    }
}
