using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecommendationEngine.Server.Migrations
{
    public partial class UpdateModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Voting",
                table: "Recommendation",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Voting",
                table: "Recommendation");
        }
    }
}
