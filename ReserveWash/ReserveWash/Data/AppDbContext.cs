using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReserveWash.Models;
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext()
    {

    }
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    

    public DbSet<Car> Car { get; set; }
    public DbSet<Carwash> Carwash { get; set; }
    public DbSet<Feedback> Feedback { get; set; }
    public DbSet<Reservation> Reservation { get; set; }
    public DbSet<Service> Service { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Carwash>()
            .HasMany(c => c.Services)
            .WithOne(s => s.Carwash)
            .HasForeignKey(s => s.CarwashId);

        modelBuilder.Entity<Carwash>()
            .HasMany(c => c.Feedbacks)
            .WithOne(f => f.Carwash)
            .HasForeignKey(f => f.CarwashId);
    }
}