using CommentsAPI.Commands;
using CommentsAPI.CustomExceptions;
using CommentsAPI.Data.Entities;
using CommentsAPI.Data;
using MediatR;

namespace CommentsAPI.Handlers
{
    internal class CreateCommentHandler : IRequestHandler<CreateCommentCommand, int>
    {
        private readonly AppDbContext _context;

        public CreateCommentHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Comment))
            {
                throw new DataProcessingException(System.Net.HttpStatusCode.BadRequest,
                    $"Comment can not be null or empty.");
            }

            var comment = new Comment
            {
                Content = request.Comment
            };

            await _context.AddAsync(comment);
            await _context.SaveChangesAsync(cancellationToken);

            return comment.Id;
        }
    }
}
