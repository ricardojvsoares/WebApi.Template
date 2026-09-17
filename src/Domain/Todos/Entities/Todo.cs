using Domain.Common;

namespace Domain.Todos.Entities;

public sealed class Todo : Entity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? DueDateUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public Guid OwnerUserId { get; set; }
}
