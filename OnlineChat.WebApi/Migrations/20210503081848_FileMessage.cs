using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineChat.WebApi.Migrations
{
    public partial class FileMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                table: "MessageContents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Filename",
                table: "MessageContents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "MessageContents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileId",
                table: "MessageContents");

            migrationBuilder.DropColumn(
                name: "Filename",
                table: "MessageContents");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "MessageContents");
        }
    }
}
