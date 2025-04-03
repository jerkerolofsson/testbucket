using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class JwtAudienceIssuer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JwtAudience",
                table: "GlobalSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JwtIssuer",
                table: "GlobalSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SymmetricJwtKey",
                table: "GlobalSettings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JwtAudience",
                table: "GlobalSettings");

            migrationBuilder.DropColumn(
                name: "JwtIssuer",
                table: "GlobalSettings");

            migrationBuilder.DropColumn(
                name: "SymmetricJwtKey",
                table: "GlobalSettings");
        }
    }
}
