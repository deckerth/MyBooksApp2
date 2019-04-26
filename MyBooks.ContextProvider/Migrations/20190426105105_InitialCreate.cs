using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyBooks.ContextProvider.Migrations
{
    public partial class InitialCreate : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    OriginalTitle = table.Column<string>(nullable: true),
                    Authors = table.Column<string>(nullable: true),
                    Keywords = table.Column<string>(nullable: true),
                    Medium = table.Column<int>(nullable: false),
                    Storage = table.Column<string>(nullable: true),
                    BorrowedDate = table.Column<string>(nullable: true),
                    BorrowedTo = table.Column<string>(nullable: true),
                    Published = table.Column<string>(nullable: true),
                    OCLCNo = table.Column<string>(nullable: true),
                    DLCNo = table.Column<string>(nullable: true),
                    ISBN = table.Column<string>(nullable: true),
                    NBACN = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    GoogleBooksUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Keywords",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keywords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Storages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Keywords");

            migrationBuilder.DropTable(
                name: "Storages");
        }
    }
}
