using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TaskService.Application.Mappings;
using TaskService.Contracts;
using TaskService.Infrastructure.Data;

namespace TaskService.Application.Services;

public class CommentService(ApplicationDbContext dbContext) : Contracts.CommentService.CommentServiceBase
{
    public override async Task<GetCommentsByTaskIdResponse> GetManyByTaskId(GetCommentsByTaskIdRequest request, ServerCallContext context)
    {
        var taskId = Guid.Parse(request.TaskId);
        var comments = await dbContext.Comments
            .Where(c => c.TaskId == taskId)
            .ToListAsync(context.CancellationToken);

        var response = new GetCommentsByTaskIdResponse();
        response.Comments.AddRange(comments.Select(c => c.ToDto()));
        
        return response;
    }

    public override async Task<TaskCommentDto> GetById(GetCommentByIdRequest request, ServerCallContext context)
    {
        var commentId = Guid.Parse(request.CommentId);
        var comment = await dbContext.Comments
            .FirstOrDefaultAsync(c => c.Id == commentId, context.CancellationToken);

        if (comment == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Comment with ID {commentId} not found."));
        }

        return comment.ToDto();
    }

    public override async Task<TaskCommentDto> Create(CreateCommentRequest request, ServerCallContext context)
    {
        var newComment = new Domain.Entities.CommentEntity
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            TaskId = Guid.Parse(request.TaskId),
            CreatedByUserId = Guid.Parse(request.CreatedByUserId),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Comments.Add(newComment);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        return newComment.ToDto();
    }

    public override async Task<TaskCommentDto> Update(UpdateCommentRequest request, ServerCallContext context)
    {
        var commentId = Guid.Parse(request.CommentId);
        var comment = await dbContext.Comments
            .FirstOrDefaultAsync(c => c.Id == commentId, context.CancellationToken);

        if (comment == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Comment with ID {commentId} not found."));
        }

        comment.Content = request.Content;
        dbContext.Comments.Update(comment);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        return comment.ToDto();
    }

    public override async Task<Empty> Delete(DeleteCommentRequest request, ServerCallContext context)
    {
        var commentId = Guid.Parse(request.CommentId);
        var comment = await dbContext.Comments
            .FirstOrDefaultAsync(c => c.Id == commentId, context.CancellationToken);

        if (comment == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Comment with ID {commentId} not found."));
        }

        dbContext.Comments.Remove(comment);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        return new Empty();
    }
}