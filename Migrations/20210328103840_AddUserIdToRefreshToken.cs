using Microsoft.EntityFrameworkCore.Migrations;

namespace PMAuth.Migrations
{
    public partial class AddUserIdToRefreshToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "RefreshTokens",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RefreshTokens");
        }
    }
}
