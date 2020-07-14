using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineChat.WebApi.Migrations
{
    public partial class ModelsCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Discriminator = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OwnerNickname = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chats_Users_OwnerNickname",
                        column: x => x.OwnerNickname,
                        principalTable: "Users",
                        principalColumn: "Nickname",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageContents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Discriminator = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatMember",
                columns: table => new
                {
                    ChatId = table.Column<int>(nullable: false),
                    Username = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMember", x => new { x.ChatId, x.Username });
                    table.ForeignKey(
                        name: "FK_ChatMember_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMember_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Nickname",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorNickname = table.Column<string>(nullable: true),
                    ContentId = table.Column<int>(nullable: true),
                    SentOn = table.Column<DateTime>(nullable: false),
                    HideForAuthor = table.Column<bool>(nullable: false),
                    ChatId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Users_AuthorNickname",
                        column: x => x.AuthorNickname,
                        principalTable: "Users",
                        principalColumn: "Nickname",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_MessageContents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "MessageContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMember_Username",
                table: "ChatMember",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_OwnerNickname",
                table: "Chats",
                column: "OwnerNickname");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_AuthorNickname",
                table: "Messages",
                column: "AuthorNickname");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ContentId",
                table: "Messages",
                column: "ContentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMember");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "MessageContents");
        }
    }
}
