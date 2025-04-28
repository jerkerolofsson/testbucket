using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class RepositoryExternalSystemId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ExternalSystemId",
                table: "Repositories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_ExternalSystemId",
                table: "Repositories",
                column: "ExternalSystemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Repositories_external_systems_ExternalSystemId",
                table: "Repositories",
                column: "ExternalSystemId",
                principalTable: "external_systems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repositories_external_systems_ExternalSystemId",
                table: "Repositories");

            migrationBuilder.DropIndex(
                name: "IX_Repositories_ExternalSystemId",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "ExternalSystemId",
                table: "Repositories");
        }
    }
}
