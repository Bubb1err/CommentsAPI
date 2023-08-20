using CommentsAPI.CustomExceptions;
using CommentsAPI.Data.Entities;
using CommentsAPI.Data;
using CommentsAPI.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CommentsAPI.Handlers
{
    internal class GetCommnetHandler : IRequestHandler<GetCommentQuery, Comment>
    {
        private readonly AppDbContext _context;

        public GetCommnetHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Comment> Handle(GetCommentQuery request, CancellationToken cancellationToken)
        {
            var comment = await _context.Set<Comment>().FirstOrDefaultAsync(c => c.Id == request.Id);
            if (comment == null)
            {
                throw new DataProcessingException(System.Net.HttpStatusCode.NotFound,
                    $"Comment with id {request.Id} was not found.");
            }
            return comment;
        }
    }
}
