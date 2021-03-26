using Microsoft.EntityFrameworkCore.Migrations;

namespace PMAuth.Migrations
{
    public partial class AddLogoPathToSocial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoPath",
                table: "Socials",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoPath",
                table: "Socials");
        }
    }
}
