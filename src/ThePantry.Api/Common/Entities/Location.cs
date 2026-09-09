namespace ThePantry.Api.Common.Entities;

public class Location
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;
}