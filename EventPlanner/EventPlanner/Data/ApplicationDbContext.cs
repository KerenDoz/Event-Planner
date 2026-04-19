using EventPlanner.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Location> Locations { get; set; } = null!;
    public DbSet<Participant> Participants { get; set; } = null!;

    //public DbSet<Review> Reviews { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>().HasIndex(c => c.Name).IsUnique();

        builder.Entity<Participant>().HasKey(p => new { p.UserId, p.EventId });

        builder
            .Entity<Participant>()
            .HasOne(p => p.User)
            .WithMany(u => u.Participants)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Entity<Participant>()
            .HasOne(p => p.Event)
            .WithMany(e => e.Participants)
            .HasForeignKey(p => p.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Entity<Event>()
            .HasOne(e => e.Organizer)
            .WithMany(u => u.CreatedEvents)
            .HasForeignKey(e => e.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
