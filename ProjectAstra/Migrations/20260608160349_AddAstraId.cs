using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAstra.Migrations
{
    /// <inheritdoc />
    public partial class AddAstraId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AstraId",
                table: "Users",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AstraId",
                table: "Users");
        }
    }
}
