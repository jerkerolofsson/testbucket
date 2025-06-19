using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLabRepoFolders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FolderId",
                table: "testsuites",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FolderId",
                table: "runs",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "testlab__folders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: true),
                    PathIds = table.Column<long[]>(type: "bigint[]", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testlab__folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_testlab__folders_testlab__folders_ParentId",
                        column: x => x.ParentId,
                        principalTable: "testlab__folders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "testrepository__folders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: true),
                    PathIds = table.Column<long[]>(type: "bigint[]", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testrepository__folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_testrepository__folders_testrepository__folders_ParentId",
                        column: x => x.ParentId,
                        principalTable: "testrepository__folders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_testsuites_FolderId",
                table: "testsuites",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_runs_FolderId",
                table: "runs",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_testlab__folders_ParentId",
                table: "testlab__folders",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_testrepository__folders_ParentId",
                table: "testrepository__folders",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_runs_testlab__folders_FolderId",
                table: "runs",
                column: "FolderId",
                principalTable: "testlab__folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_testsuites_testrepository__folders_FolderId",
                table: "testsuites",
                column: "FolderId",
                principalTable: "testrepository__folders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_runs_testlab__folders_FolderId",
                table: "runs");

            migrationBuilder.DropForeignKey(
                name: "FK_testsuites_testrepository__folders_FolderId",
                table: "testsuites");

            migrationBuilder.DropTable(
                name: "testlab__folders");

            migrationBuilder.DropTable(
                name: "testrepository__folders");

            migrationBuilder.DropIndex(
                name: "IX_testsuites_FolderId",
                table: "testsuites");

            migrationBuilder.DropIndex(
                name: "IX_runs_FolderId",
                table: "runs");

            migrationBuilder.DropColumn(
                name: "FolderId",
                table: "testsuites");

            migrationBuilder.DropColumn(
                name: "FolderId",
                table: "runs");
        }
    }
}
