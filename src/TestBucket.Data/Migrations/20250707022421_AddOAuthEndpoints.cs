using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOAuthEndpoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthEndpoint",
                table: "external_systems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenEndpoint",
                table: "external_systems",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthEndpoint",
                table: "external_systems");

            migrationBuilder.DropColumn(
                name: "TokenEndpoint",
                table: "external_systems");
        }
    }
}
