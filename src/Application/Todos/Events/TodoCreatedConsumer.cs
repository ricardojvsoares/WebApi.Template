using Microsoft.Extensions.Logging;

namespace Application.Todos.Events;

/// <summary>
/// Worked example of a message consumer. Runs when the event is delivered from the
/// RabbitMQ queue, not in the request that published it.
/// </summary>
public static class TodoCreatedConsumer
{
    public static void Consume(
        TodoCreatedEvent message,
        ILogger logger)
    {
        logger.LogInformation(
            "Consumed {EventName} for todo {TodoId} ('{Title}') owned by {OwnerUserId}",
            nameof(TodoCreatedEvent),
            message.TodoId,
            message.Title,
            message.OwnerUserId);
    }
}
