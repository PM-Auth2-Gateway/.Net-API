using Microsoft.EntityFrameworkCore.Migrations;

namespace PMAuth.Migrations
{
    public partial class AddAuthUriToSocial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthUri",
                table: "Socials",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthUri",
                table: "Socials");
        }
    }
}
