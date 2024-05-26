using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCA.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOtherCategoryToChallenges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtherCategory",
                table: "Challenges",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherCategory",
                table: "Challenges");
        }
    }
}
