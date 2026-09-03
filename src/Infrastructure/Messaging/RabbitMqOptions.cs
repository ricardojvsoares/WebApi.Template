namespace Infrastructure.Messaging;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    /// <summary>
    /// When false, no transport is configured and publishing an integration event becomes
    /// a no-op, so the API still runs with no broker available.
    /// </summary>
    public bool Enabled { get; set; }

    public string Host { get; set; } = "localhost";

    public int Port { get; set; } = 5672;

    public string Username { get; set; } = "guest";

    public string Password { get; set; } = "guest";

    public string VirtualHost { get; set; } = "/";

    /// <summary>Exchange that todo integration events are published to.</summary>
    public string TodosExchange { get; set; } = "webapi.todos";

    /// <summary>Queue this API listens on for todo integration events.</summary>
    public string TodosQueue { get; set; } = "webapi.todos.notifications";
}
