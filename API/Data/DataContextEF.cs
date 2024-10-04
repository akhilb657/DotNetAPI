using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data
{
  public class DataContextEF : DbContext
  {
    private readonly IConfiguration _congif;

    public DataContextEF(IConfiguration config)
    {
      _congif = config;
    }

    public virtual DbSet<User> Users {get; set;}
    public virtual DbSet<UserSalary> UserSalary {get; set;}
    public virtual DbSet<UserJobInfo> UserJobInfo {get; set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if(!optionsBuilder.IsConfigured)
      {
        optionsBuilder
          .UseSqlServer(_congif.GetConnectionString("DefaultConnection"),
            optionsBuilder => optionsBuilder.EnableRetryOnFailure());
      }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.HasDefaultSchema("TutorialAppSchema");

      modelBuilder.Entity<User>()
        .ToTable("Users", "TutorialAppSchema")
        .HasKey(u => u.UserId);
      
      modelBuilder.Entity<UserSalary>()
        .HasKey(u => u.UserId);

      modelBuilder.Entity<UserJobInfo>()
        .HasKey(u => u.UserId);
    }
  }
}