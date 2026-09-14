namespace ThePantry.Api.Common.Entities;

public class ProductHistory
{
    public Guid Id { get; set; }
    public required string ProductName { get; set; }
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;
    public decimal Quantity { get; set; }
    public Unit Unit { get; set; }
    public ProductHistoryAction Action { get; set; }
    public DateTime Timestamp { get; set; }
}

public enum ProductHistoryAction
{
    Consumed,
    ThrownOut,
    Expired
}