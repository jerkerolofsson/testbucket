using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class apikeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserApiKey_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserApiKey");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUserApiKey",
                table: "ApplicationUserApiKey");

            migrationBuilder.RenameTable(
                name: "ApplicationUserApiKey",
                newName: "ApiKeys");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUserApiKey_ApplicationUserId",
                table: "ApiKeys",
                newName: "IX_ApiKeys_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiKeys",
                table: "ApiKeys",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiKeys_AspNetUsers_ApplicationUserId",
                table: "ApiKeys",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiKeys_AspNetUsers_ApplicationUserId",
                table: "ApiKeys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiKeys",
                table: "ApiKeys");

            migrationBuilder.RenameTable(
                name: "ApiKeys",
                newName: "ApplicationUserApiKey");

            migrationBuilder.RenameIndex(
                name: "IX_ApiKeys_ApplicationUserId",
                table: "ApplicationUserApiKey",
                newName: "IX_ApplicationUserApiKey_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUserApiKey",
                table: "ApplicationUserApiKey",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserApiKey_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserApiKey",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
