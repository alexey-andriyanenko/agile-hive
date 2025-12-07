namespace TaskService.Domain.Entities;

public class TaskTagEntity
{
    public Guid TaskId { get; set; }
    public TaskEntity? Task { get; set; }

    public Guid TagId { get; set; }
}
