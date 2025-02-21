using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProjectMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TestProjectId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TestProjectId",
                table: "AspNetUsers",
                column: "TestProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_projects_TestProjectId",
                table: "AspNetUsers",
                column: "TestProjectId",
                principalTable: "projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_projects_TestProjectId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TestProjectId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TestProjectId",
                table: "AspNetUsers");
        }
    }
}
