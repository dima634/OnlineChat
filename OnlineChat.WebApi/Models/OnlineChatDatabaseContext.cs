using Microsoft.EntityFrameworkCore;

namespace OnlineChat.WebApi.Models
{
    public class OnlineChatDatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<DirectChat> DirectChats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ReplyMessage> ReplyMessages { get; set; }
        public DbSet<MessageContent> MessageContents { get; set; }
        public DbSet<TextContent> TextContents { get; set; }
        public DbSet<FileContent> FileContents { get; set; }
        public DbSet<MessageReadStatus> MessagesReadStatuse { get; set; }

        public OnlineChatDatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatMember>().HasKey(cm => new { cm.ChatId, cm.Username });

            modelBuilder.Entity<ChatMember>()
               .HasOne(cm => cm.User)
               .WithMany(user => user.ChatMembers)
               .HasForeignKey(cm => cm.Username);


            modelBuilder.Entity<ChatMember>()
               .HasOne(cm => cm.Chat)
               .WithMany(c => c.ChatMembers)
               .HasForeignKey(cm => cm.ChatId);

            modelBuilder.Entity<MessageReadStatus>().HasKey(mrs => new { mrs.MessageId, mrs.Username });

            modelBuilder.Entity<MessageReadStatus>()
                .HasOne(mrs => mrs.User)
                .WithMany()
                .HasForeignKey(mrs => mrs.Username);

            modelBuilder.Entity<MessageReadStatus>()
                .HasOne(mrs => mrs.Message)
                .WithMany(message => message.MessagesReadStatus)
                .HasForeignKey(mrs => mrs.MessageId);
        }
    }
}
