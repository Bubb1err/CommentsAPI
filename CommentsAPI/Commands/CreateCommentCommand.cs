using MediatR;

namespace CommentsAPI.Commands
{
    public class CreateCommentCommand : IRequest<int>
    {
        public string Comment { get; private set; }
        public CreateCommentCommand(string comment)
        {
            Comment = comment;
        }
    }
}
