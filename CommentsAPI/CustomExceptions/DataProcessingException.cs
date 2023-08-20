using System.Net;

namespace CommentsAPI.CustomExceptions
{
    public class DataProcessingException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public string ErrorMessage { get; private set; }
        public DataProcessingException(HttpStatusCode statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}
