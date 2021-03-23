using Microsoft.EntityFrameworkCore.Migrations;

namespace PMAuth.Migrations
{
    public partial class AddTokenUriToSocial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TokenUrl",
                table: "Socials",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenUrl",
                table: "Socials");
        }
    }
}
