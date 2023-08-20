using CommentsAPI.Commands;
using MediatR;

namespace CommentsAPI.Helpers
{
    public class CreateCommentHelper
    {
        private readonly IMediator _mediator;

        public CreateCommentHelper(
            IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<int> SendRequest(CreateCommentCommand request)
        {
            Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(10, 15)));
            int commentId = await _mediator.Send(request);

            return commentId;
        }
    }
}
