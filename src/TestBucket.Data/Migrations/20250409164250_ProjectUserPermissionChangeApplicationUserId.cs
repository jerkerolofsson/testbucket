using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProjectUserPermissionChangeApplicationUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUserPermissions_AspNetUsers_ApplicationUserId1",
                table: "ProjectUserPermissions");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUserPermissions_ApplicationUserId1",
                table: "ProjectUserPermissions");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "ProjectUserPermissions");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ProjectUserPermissions",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUserPermissions_ApplicationUserId",
                table: "ProjectUserPermissions",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUserPermissions_AspNetUsers_ApplicationUserId",
                table: "ProjectUserPermissions",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUserPermissions_AspNetUsers_ApplicationUserId",
                table: "ProjectUserPermissions");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUserPermissions_ApplicationUserId",
                table: "ProjectUserPermissions");

            migrationBuilder.AlterColumn<long>(
                name: "ApplicationUserId",
                table: "ProjectUserPermissions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "ProjectUserPermissions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUserPermissions_ApplicationUserId1",
                table: "ProjectUserPermissions",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUserPermissions_AspNetUsers_ApplicationUserId1",
                table: "ProjectUserPermissions",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
