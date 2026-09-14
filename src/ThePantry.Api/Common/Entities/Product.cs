namespace ThePantry.Api.Common.Entities;

public class Product
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public Guid LocationId { get; set; }
    public Location Location { get; set; } = null!;
    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;
    public decimal Quantity { get; set; }
    public Unit Unit { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsExpired { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum Unit
{
    Piece,
    Gram,
    Kilogram,
    Milliliter,
    Liter
}