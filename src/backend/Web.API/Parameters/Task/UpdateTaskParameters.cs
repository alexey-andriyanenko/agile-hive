namespace Web.API.Parameters.Task;

public class UpdateTaskParameters : CreateTaskParameters
{
    public Guid TaskId { get; set; }
}