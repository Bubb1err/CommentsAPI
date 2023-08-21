using CommentsAPI.Commands;
using CommentsAPI.Helpers;
using CommentsAPI.Queries;
using Hangfire;
using Hangfire.Storage.Monitoring;
using Hangfire.Storage;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using CommentsAPI.Data.Entities;
using CommentsAPI.Models;
using CommentsAPI.Services.LoggerService;

namespace CommentsAPI.Controllers
{
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerManager _loggerManager;
        public CommentsController(
            IMediator mediator,
            ILoggerManager loggerManager)
        {
            _mediator = mediator;
            _loggerManager = loggerManager;
        }
        /// <summary>
        /// Average time to get result of 10 requests ~ 2 minutes. Specify less requests quantity for faster result or see logs.
        /// </summary>
        /// <returns>List of requests and list of comments.</returns>
        [HttpGet("test")]
        public async Task<IActionResult> Test(int requestsCount)
        {
            var jobs = new List<JobDTO>();

            for (int i = 0; i < requestsCount; i++)
            {
                IActionResult createResponse = CreateComment($"Comment {i}.");
                var createResult = createResponse as ObjectResult;

                if (createResult != null && createResult.StatusCode == (int)HttpStatusCode.Accepted)
                {
                    string jobId = createResult.Value.ToString();
                    jobs.Add(new JobDTO
                    {
                        Id = jobId,
                        Received = DateTime.Now
                    });
                    _loggerManager.LogInfo($"New request with id {jobId} received at {DateTime.Now}.");
                }
            }

            var comments = new List<CommentDTO>();

            foreach (var jobId in jobs)
            {
                while(true)
                {
                    IActionResult requestResponse = CheckRequest(jobId.Id);
                    var requestResult = requestResponse as ObjectResult;
                    if (requestResult != null && requestResult.StatusCode == (int)HttpStatusCode.Accepted)
                    {
                        await Task.Delay(1000);
                        continue;
                    }
                    else if (requestResult != null && requestResult.StatusCode == (int)HttpStatusCode.OK) 
                    {
                        int commentId = (int)requestResult.Value;
                        IActionResult commentResponse = await GetComment(commentId);
                        var commentResult = commentResponse as ObjectResult;

                        if (commentResult != null && commentResult.StatusCode == (int)HttpStatusCode.OK)
                        {
                            var comment = commentResult.Value as Comment;
                            comments.Add(new CommentDTO
                            {
                                Comment = comment,
                                Received = DateTime.Now
                            });
                            _loggerManager.LogInfo($"Id: {comment.Id} Comment: {comment.Content} Received at: {DateTime.Now}.");
                        }
                        break;
                    }
                    else { break; }
                }
            }

            var result = new TestRequestResultDTO
            {
                JobDTOs = jobs,
                CommentDTOs = comments
            };

            return Ok(result);
        }
        
        [HttpGet("comment")]
        public async Task<IActionResult> GetComment(int id)
        {
            if (id == 0)
            {
                return BadRequest("Invalid id.");
            }
            var getCommentQuery = new GetCommentQuery(id);
            var comment = await _mediator.Send(getCommentQuery);
            return Ok(comment);

        }
        [HttpGet("check-request")]
        public IActionResult CheckRequest(string jobId)
        {
            IMonitoringApi jobMonitoringApi = JobStorage.Current.GetMonitoringApi();
            JobDetailsDto job = jobMonitoringApi.JobDetails(jobId);

            if (job.History[0].StateName == "Enqueued" || job.History[0].StateName == "Processing")
            {
                return StatusCode((int)HttpStatusCode.Accepted, jobId);
            }
            else if (job.History[0].StateName == "Succeeded")
            {
                string result = job.History[0].Data["Result"];
                int entityId = JsonConvert.DeserializeObject<int>(result);
                return Ok(entityId);
            }
            else
            {
                return BadRequest("Something went wrong.");
            }
        }
        [HttpPost("comment")]
        public IActionResult CreateComment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment)) { return BadRequest("Comment cannot be empty."); }

            var createCommentCommand = new CreateCommentCommand(comment);

            var createCommentHelper = new CreateCommentHelper(_mediator);
            string jobId = BackgroundJob.Enqueue(queue: "default", () => createCommentHelper.SendRequest(createCommentCommand));

            return StatusCode((int)HttpStatusCode.Accepted, jobId);
        }
    }
}
