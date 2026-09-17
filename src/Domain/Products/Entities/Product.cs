using Domain.Common;

namespace Domain.Products.Entities;

public sealed class Product : Entity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public float Price { get; set; }
    public Uri Image { get; set; } = null!;
}
