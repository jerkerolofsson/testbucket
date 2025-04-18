using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class RunnerLanguages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Languages",
                table: "Runners",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Languages",
                table: "Runners");
        }
    }
}
