using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TestBucket.Contracts.Testing.Models;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProjectTestStateJsonb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestState");

            migrationBuilder.AddColumn<TestState[]>(
                name: "TestStates",
                table: "projects",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestStates",
                table: "projects");

            migrationBuilder.CreateTable(
                name: "TestState",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsFinal = table.Column<bool>(type: "boolean", nullable: false),
                    IsInitial = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    TestProjectId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestState", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestState_projects_TestProjectId",
                        column: x => x.TestProjectId,
                        principalTable: "projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestState_TestProjectId",
                table: "TestState",
                column: "TestProjectId");
        }
    }
}
