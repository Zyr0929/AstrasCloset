using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectAstra.Migrations
{
    /// <inheritdoc />
    public partial class ReviewImp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerReview");

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "OrderItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "OrderItems");

            migrationBuilder.CreateTable(
                name: "CustomerReview",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApparelProductId = table.Column<int>(type: "int", nullable: true),
                    FitStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RatingScore = table.Column<int>(type: "int", nullable: false),
                    ReviewBodyText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReviewerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SelectedColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SelectedSize = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerReview_Products_ApparelProductId",
                        column: x => x.ApparelProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReview_ApparelProductId",
                table: "CustomerReview",
                column: "ApparelProductId");
        }
    }
}
