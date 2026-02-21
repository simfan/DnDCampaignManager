using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Models;

namespace BlazorApp1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Your DbSets
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterClass> CharacterClasses { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatRoomMember> ChatRoomMembers { get; set; }
        public DbSet<CampaignMember> CampaignMembers { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Journal> Journals { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Tag> Tags { get; set;  }
        public DbSet<PlayerEvent> PlayerEvents { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceShare> ResourceShares { get; set;  }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // IMPORTANT: calls Identity configuration

            // Campaign Configuration
            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.HasKey(e => e.CampaignId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(e => e.CreatedBy)
                    .WithMany(u => u.CreatedCampaigns)
                    .HasForeignKey(e => e.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Ignore(e => e.Journal);
            });

            // Character Configuration
            modelBuilder.Entity<Character>(entity =>
            {
                entity.HasKey(e => e.CharacterId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(e => e.Campaign)
                    .WithMany(c => c.Characters)
                    .HasForeignKey(e => e.CampaignId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Player)
                    .WithMany(u => u.Characters)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CharacterClass Configuration
            modelBuilder.Entity<CharacterClass>(entity =>
            {
                entity.HasKey(e => e.CharacterClassId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);

                entity.HasOne(e => e.Character)
                    .WithMany(c => c.Classes)
                    .HasForeignKey(e => e.CharacterId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ChatRoom Configuration
            modelBuilder.Entity<ChatRoom>(entity =>
            {
                entity.HasKey(e => e.ChatRoomId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(e => e.CreatedBy)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Campaign)
                    .WithMany()
                    .HasForeignKey(e => e.CampaignId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ChatMessage Configuration
            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.ChatMessageId);
                entity.Property(e => e.Content).IsRequired();

                entity.HasOne(e => e.ChatRoom)
                    .WithMany(r => r.Messages)
                    .HasForeignKey(e => e.ChatRoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Sender)
                    .WithMany()
                    .HasForeignKey(e => e.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ChatRoomMember Configuration
            modelBuilder.Entity<ChatRoomMember>(entity =>
            {
                entity.HasKey(e => e.ChatRoomMemberId);
                entity.HasIndex(e => new { e.UserId, e.ChatRoomId }).IsUnique();

                entity.HasOne(e => e.ChatRoom)
                    .WithMany(r => r.Members)
                    .HasForeignKey(e => e.ChatRoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CampaignMember Configuration
            modelBuilder.Entity<CampaignMember>(entity =>
            {
                entity.HasKey(e => e.CampaignMemberId);
                entity.HasIndex(e => new { e.CampaignId, e.UserId }).IsUnique();

                entity.HasOne(e => e.Campaign)
                    .WithMany(c => c.Members)
                    .HasForeignKey(e => e.CampaignId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.CampaignMemberships)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Entry>(entity =>
            { 
                entity.HasKey(e =>  e.EntryId); 
                
                entity.HasOne(e => e.Journal)
                .WithMany(j => j.Entries)
                .HasForeignKey(e => e.JournalId)
                .OnDelete(DeleteBehavior.Cascade);

                /*entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);*/
            });

            modelBuilder.Entity<EntryTag>(entity =>
            {
                entity.HasKey(et => new { et.EntryId, et.TagId });

                entity.HasOne(et => et.Entry)
                .WithMany(e => e.Tags)
                .HasForeignKey(et => et.EntryId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(et => et.Tag)
                .WithMany(e => e.EntryTags)
                .HasForeignKey(et => et.TagId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<InventoryItem>(entity =>
            {
                entity.HasKey(e => e.InventoryItemId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

                // TPH discriminator
                entity.HasDiscriminator<ItemType>("ItemType")
                    .HasValue<InventoryItem>(ItemType.Generic)
                    .HasValue<WeaponItem>(ItemType.Weapon)
                    .HasValue<ArmorItem>(ItemType.Armor)
                    .HasValue<MagicItem>(ItemType.MagicItem)
                    .HasValue<PotionItem>(ItemType.Potion)
                    .HasValue<AmmunitionItem>(ItemType.Ammunition)
                    .HasValue<ToolItem>(ItemType.Tool)
                    .HasValue<MountItem>(ItemType.Mount);

                entity.HasOne(e => e.Character)
                    .WithMany()
                    .HasForeignKey(e => e.CharacterId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Campaign)
                    .WithMany()
                    .HasForeignKey(e => e.CampaignId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<JournalTag>(entity =>
            {
                entity.HasKey(jt => new { jt.JournalId, jt.TagId }); // Composite key

                entity.HasOne(jt => jt.Journal)
                    .WithMany(j => j.Tags)
                    .HasForeignKey(jt => jt.JournalId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(jt => jt.Tag)
                    .WithMany(t => t.JournalTags)
                    .HasForeignKey(jt => jt.TagId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(e => e.ResourceId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(1000);

                entity.HasOne(e => e.Campaign)
                    .WithMany()
                    .HasForeignKey(e => e.CampaignId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.UploadedBy)
                    .WithMany()
                    .HasForeignKey(e => e.UploadedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ResourceShare>(entity =>
            {
                entity.HasKey(e => e.ResourceId);

                entity.HasOne(e => e.Resource)
                    .WithMany(r => r.Shares)
                    .HasForeignKey(e => e.ResourceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e =>e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Character)
                .WithMany()
                .HasForeignKey(e => e.CharacterId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.SharedBy)
                .WithMany()
                .HasForeignKey(e => e.SharedById)
                .OnDelete(DeleteBehavior.Restrict);

            });
        }
    }
}