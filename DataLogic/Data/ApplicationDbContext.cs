using Microsoft.EntityFrameworkCore;
using ModelLayer.Entities;

namespace DataLogic.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<Label> Labels { get; set; }
    public DbSet<Collaborator> Collaborators { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId)
                .UseIdentityColumn();

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_User_Email_Unique");

            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            
            entity.HasMany(u => u.Notes)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(u => u.Labels)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(U => U.Collaborators)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(e => e.NoteId);

                entity.Property(e => e.NoteId)
                    .UseIdentityColumn();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Description)
                    .HasMaxLength(-1);

                entity.Property(e => e.Colour)
                    .HasMaxLength(7)
                    .HasDefaultValue("#FFFFFF");

                entity.Property(e => e.Image)
                    .HasMaxLength(-1);

                entity.Property(e => e.IsArchive)
                    .HasDefaultValue(0);

                entity.Property(e => e.IsPin)
                    .HasDefaultValue(0);

                entity.Property(e => e.IsTrash)
                    .HasDefaultValue(0);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.ChangedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasMany(u => u.Labels)
                    .WithMany(l => l.Notes)
                    .UsingEntity(j => j.ToTable("NoteLabels"));
            }
        );

        modelBuilder.Entity<Label>(entity =>
            {
                entity.HasKey(e => e.LabelId);

                entity.Property(e => e.LabelId)
                    .UseIdentityColumn();

                entity.Property(e => e.LabelName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.ChangedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            }
        );

        modelBuilder.Entity<Collaborator>(entity =>
            {
                entity.HasKey(e => e.CollaboratorId);

                entity.Property(e => e.CollaboratorId)
                    .UseIdentityColumn();

                entity.Property(e => e.CollaboratorId);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(c => c.Note)
                    .WithMany(c => c.Collaborators)
                    .HasForeignKey(c => c.NoteId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        );
    }
}