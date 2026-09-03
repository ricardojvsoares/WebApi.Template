namespace Domain.Todos.Entities;

public sealed class Todo
{
    private Todo() { }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? DueDateUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public Guid OwnerUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public static Todo Create(
        string title,
        string? description,
        DateTime? dueDateUtc,
        Guid ownerUserId,
        DateTime nowUtc)
    {
        return new Todo
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            DueDateUtc = dueDateUtc,
            OwnerUserId = ownerUserId,
            IsCompleted = false,
            CompletedAtUtc = null,
            CreatedAtUtc = nowUtc,
            UpdatedAtUtc = nowUtc
        };
    }

    public void Update(
        string title,
        string? description,
        DateTime? dueDateUtc,
        DateTime nowUtc)
    {
        Title = title;
        Description = description;
        DueDateUtc = dueDateUtc;
        UpdatedAtUtc = nowUtc;
    }

    /// <summary>
    /// Marks the todo complete. Returns false when it was already complete, so callers
    /// can report a conflict rather than silently overwriting the completion timestamp.
    /// </summary>
    public bool Complete(
        DateTime nowUtc)
    {
        if (IsCompleted)
        {
            return false;
        }

        IsCompleted = true;
        CompletedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;

        return true;
    }

    public bool Reopen(
        DateTime nowUtc)
    {
        if (!IsCompleted)
        {
            return false;
        }

        IsCompleted = false;
        CompletedAtUtc = null;
        UpdatedAtUtc = nowUtc;

        return true;
    }
}
