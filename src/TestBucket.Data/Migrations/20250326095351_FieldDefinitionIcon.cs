using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBucket.Data.Migrations
{
    /// <inheritdoc />
    public partial class FieldDefinitionIcon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "field_definitions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Dictionary<string, string>>(
                name: "OptionIcons",
                table: "field_definitions",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "field_definitions");

            migrationBuilder.DropColumn(
                name: "OptionIcons",
                table: "field_definitions");
        }
    }
}
