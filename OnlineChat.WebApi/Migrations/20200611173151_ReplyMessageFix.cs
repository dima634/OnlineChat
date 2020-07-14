using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineChat.WebApi.Migrations
{
    public partial class ReplyMessageFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Messages",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ReplyToId",
                table: "Messages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReplyToId",
                table: "Messages",
                column: "ReplyToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_ReplyToId",
                table: "Messages",
                column: "ReplyToId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_ReplyToId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ReplyToId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ReplyToId",
                table: "Messages");
        }
    }
}
