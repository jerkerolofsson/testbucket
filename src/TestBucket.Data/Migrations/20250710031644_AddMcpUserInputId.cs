using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMcpUserInputId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InputId",
                table: "McpServerUserInputs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InputId",
                table: "McpServerUserInputs");
        }
    }
}
