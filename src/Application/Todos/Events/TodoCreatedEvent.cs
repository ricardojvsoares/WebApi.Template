namespace Application.Todos.Events;

/// <summary>
/// Integration event published after a todo is created. Routed to RabbitMQ when the
/// broker is enabled; publishing is a no-op otherwise.
/// </summary>
public sealed record TodoCreatedEvent(
    Guid TodoId,
    string Title,
    Guid OwnerUserId,
    DateTime CreatedAtUtc);
