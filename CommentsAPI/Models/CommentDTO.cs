using CommentsAPI.Data.Entities;

namespace CommentsAPI.Models
{
    public class CommentDTO
    {
        public Comment Comment { get; set; }
        public DateTime Received { get; set; }
    }
}
