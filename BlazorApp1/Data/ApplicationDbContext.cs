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
        public DbSet<CampaignMember> CampaignMembers { get; set; }

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
        }
    }
}