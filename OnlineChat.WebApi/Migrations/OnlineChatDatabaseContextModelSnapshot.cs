﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OnlineChat.WebApi.Models;

namespace OnlineChat.WebApi.Migrations
{
    [DbContext(typeof(OnlineChatDatabaseContext))]
    partial class OnlineChatDatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("OnlineChat.WebApi.Models.Chat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Chats");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Chat");
                });

            modelBuilder.Entity("OnlineChat.WebApi.Models.ChatMember", b =>
                {
                    b.Property<int>("ChatId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ChatId", "Username");

                    b.HasIndex("Username");

                    b.ToTable("ChatMember");
                });

            modelBuilder.Entity("OnlineChat.WebApi.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AuthorNickname")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("ChatId")
                        .HasColumnType("int");

                    b.Property<int?>("ContentId")
                        .HasColumnType("int");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("HideForAuthor")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEdited")
                        .HasColumnType("bit");

                    b.Property<DateTime>("SentOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AuthorNickname");

                    b.HasIndex("ChatId");

                    b.HasIndex("ContentId");

                    b.ToTable("Messages");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Message");
                });

            modelBuilder.Entity("OnlineChat.WebApi.Models.MessageContent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MessageContents");

                    b.HasDiscriminator<string>("Discriminator").HasValue("MessageContent");
                });

            modelBuilder.Entity("OnlineChat.WebApi.Models.MessageReadStatus", b =>
                {
                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.HasKey("MessageId", "Username");

                    b.HasIndex("Username");

                    b.ToTable("MessagesReadStatuse");
                });

            modelBuilder.Entity("OnlineChat.WebApi.Models.User", b =>
                {
                    b.Property<string>("Nickname")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Nickname");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("OnlineChat.WebApi.Models.DirectChat", b =>
                {
                    b.HasBaseType("OnlineChat.WebApi.Models.Chat");

                    b.HasDiscriminator().HasValue("DirectChat");
                });

            modelBuilder.Entity("OnlineChat.WebApi.Models.GroupChat", b =>
                {
                    b.HasBaseType("OnlineChat.WebApi.Models.Chat");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OwnerNickname")
                        .HasColumnType("nvarchar(450)");

                    b.HasIndex("OwnerNickname");

                    b.HasDiscriminator().HasValue("GroupChat");
                });

            modelBuilder.Entity("OnlineChat.WebApi.Models.ReplyMessage", b =>
                {
                    b.HasBaseType("OnlineChat.WebApi.Models.Message");

                    b.Property<int?>("ReplyToId")
                        .HasColumnType("int");

                    b.HasIndex("ReplyToId");

                    b.HasDiscriminator().HasValue("ReplyMessage");
                });

            modelBuilder.Entity("OnlineChat.WebApi.Models.TextContent", b =>
                {
                    b.HasBaseType("OnlineChat.WebApi.Models.MessageContent");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("TextContent");
                });

            modelBuilder.Entity("OnlineChat.WebApi.Models.ChatMember", b =>
                {
                    b.HasOne("OnlineChat.WebApi.Models.Chat", "Chat")
                        .WithMany("ChatMembers")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OnlineChat.WebApi.Models.User", "User")
                        .WithMany("ChatMembers")
                        .HasForeignKey("Username")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OnlineChat.WebApi.Models.Message", b =>
                {
                    b.HasOne("OnlineChat.WebApi.Models.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorNickname");

                    b.HasOne("OnlineChat.WebApi.Models.Chat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId");

                    b.HasOne("OnlineChat.WebApi.Models.MessageContent", "Content")
                        .WithMany()
                        .HasForeignKey("ContentId");
                });

            modelBuilder.Entity("OnlineChat.WebApi.Models.MessageReadStatus", b =>
                {
                    b.HasOne("OnlineChat.WebApi.Models.Message", "Message")
                        .WithMany("MessagesReadStatus")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OnlineChat.WebApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("Username")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OnlineChat.WebApi.Models.GroupChat", b =>
                {
                    b.HasOne("OnlineChat.WebApi.Models.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerNickname");
                });

            modelBuilder.Entity("OnlineChat.WebApi.Models.ReplyMessage", b =>
                {
                    b.HasOne("OnlineChat.WebApi.Models.Message", "ReplyTo")
                        .WithMany()
                        .HasForeignKey("ReplyToId");
                });
#pragma warning restore 612, 618
        }
    }
}
