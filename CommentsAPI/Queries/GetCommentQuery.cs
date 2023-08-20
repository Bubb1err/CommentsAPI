using CommentsAPI.Data.Entities;
using MediatR;

namespace CommentsAPI.Queries
{
    public class GetCommentQuery : IRequest<Comment>
    {
        public int Id { get; private set; }
        public GetCommentQuery(int id)
        {
            Id = id;
        }
    }
}
