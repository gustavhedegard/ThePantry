using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThePantry.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintOnHouseholdMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HouseholdMembers_UserId",
                table: "HouseholdMembers");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdMembers_UserId_HouseholdId",
                table: "HouseholdMembers",
                columns: new[] { "UserId", "HouseholdId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HouseholdMembers_UserId_HouseholdId",
                table: "HouseholdMembers");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdMembers_UserId",
                table: "HouseholdMembers",
                column: "UserId");
        }
    }
}
