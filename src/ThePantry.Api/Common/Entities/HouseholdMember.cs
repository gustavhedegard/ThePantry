namespace ThePantry.Api.Common.Entities;

public class HouseholdMember
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;
    public HouseholdRole Role {get; set; }

}

public enum HouseholdRole
{
    Owner,
    Member
}