using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineChat.WebApi.Migrations
{
    public partial class MessageReadStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessagesReadStatuse",
                columns: table => new
                {
                    MessageId = table.Column<int>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagesReadStatuse", x => new { x.MessageId, x.Username });
                    table.ForeignKey(
                        name: "FK_MessagesReadStatuse_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessagesReadStatuse_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Nickname",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessagesReadStatuse_Username",
                table: "MessagesReadStatuse",
                column: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessagesReadStatuse");
        }
    }
}
