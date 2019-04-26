using Microsoft.EntityFrameworkCore.Migrations;

namespace MyBooks.ContextProvider.Migrations
{
    public partial class NewColumnASIN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ASIN",
                table: "Books",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ASIN",
                table: "Books");
        }
    }
}
