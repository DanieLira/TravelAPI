using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelAPI.Migrations
{
    public partial class UpdateReviewModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_AuthorUserId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_AuthorUserId",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "AuthorUserId",
                table: "Reviews",
                newName: "AuthorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Reviews",
                newName: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AuthorUserId",
                table: "Reviews",
                column: "AuthorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_AuthorUserId",
                table: "Reviews",
                column: "AuthorUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
