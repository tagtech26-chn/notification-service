using Microsoft.EntityFrameworkCore;

namespace AnujTiles.NotificationEngine.Logging;

public sealed class NotificationDbContext
    : DbContext
{
    public NotificationDbContext(
        DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    public DbSet<NotificationLog> NotificationLogs =>
        Set<NotificationLog>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationLog>(
            entity =>
            {
                entity.ToTable("NotificationLogs");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.ApplicationName)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.Channel)
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(x => x.Recipient)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(x => x.Subject)
                    .HasMaxLength(500);

                entity.Property(x => x.TemplateName)
                    .HasMaxLength(150);

                entity.Property(x => x.Status)
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(x => x.ProviderMessageId)
                    .HasMaxLength(300);

                entity.Property(x => x.ErrorMessage)
                    .HasMaxLength(4000);

                entity.HasIndex(x => x.CreatedAt);

                entity.HasIndex(x =>
                    new
                    {
                        x.ApplicationName,
                        x.Channel,
                        x.CreatedAt
                    });
            });
    }
}