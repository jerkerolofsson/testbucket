using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class RequirementParentReadOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReadOnly",
                table: "spec",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ParentRequirementId",
                table: "requirements",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReadOnly",
                table: "requirements",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RequirementType",
                table: "requirements",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReadOnly",
                table: "spec");

            migrationBuilder.DropColumn(
                name: "ParentRequirementId",
                table: "requirements");

            migrationBuilder.DropColumn(
                name: "ReadOnly",
                table: "requirements");

            migrationBuilder.DropColumn(
                name: "RequirementType",
                table: "requirements");
        }
    }
}
