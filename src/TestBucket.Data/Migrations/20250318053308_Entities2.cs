using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class Entities2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_testsuite__folders_Tenants_TenantId",
                table: "testsuite__folders");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "testsuite__folders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_testsuite__folders_Tenants_TenantId",
                table: "testsuite__folders",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_testsuite__folders_Tenants_TenantId",
                table: "testsuite__folders");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "testsuite__folders",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_testsuite__folders_Tenants_TenantId",
                table: "testsuite__folders",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
