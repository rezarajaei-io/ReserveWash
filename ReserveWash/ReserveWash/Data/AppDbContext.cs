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
    public DbSet<Reservation> Reservation { get; set; }
    public DbSet<Service> Service { get; set; }
    public DbSet<ReserveTime> ReserveTime { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Carwash>()
            .HasMany(c => c.Services)
            .WithOne(s => s.Carwash)
            .HasForeignKey(s => s.CarwashId)
            .OnDelete(DeleteBehavior.NoAction); // یا Restrict

        modelBuilder.Entity<ReserveTime>()
            .HasOne(c => c.Service)
            .WithMany(s => s.ReserveTime)
            .HasForeignKey(s => s.ServiceId)
            .OnDelete(DeleteBehavior.NoAction); // یا Restrict

        modelBuilder.Entity<ReserveTime>()
           .HasOne(c => c.Carwash)
           .WithMany(s => s.ReserveTime)
           .HasForeignKey(s => s.CarwashId)
           .OnDelete(DeleteBehavior.NoAction); // یا Restrict

      
        modelBuilder.Entity<Carwash>()
            .HasOne(c => c.User)
            .WithMany() // اگر User به Carwash اشاره نمی‌کند
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.NoAction); // یا 

        modelBuilder.Entity<Car>()
         .HasOne(c => c.User)
         .WithMany() // اگر User به Carwash اشاره نمی‌کند
         .HasForeignKey("UserId")
         .OnDelete(DeleteBehavior.NoAction); // یا 

        modelBuilder.Entity<Reservation>()
           .HasOne(c => c.ReserveTime)
           .WithMany(s => s.Reservation)
           .HasForeignKey(s => s.ReserveTimeId)
           .OnDelete(DeleteBehavior.NoAction); // یا Restrict
    }
}