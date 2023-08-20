using System.Text.Json;

namespace CommentsAPI.Models
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public ErrorDetails(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
