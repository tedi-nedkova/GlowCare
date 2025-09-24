using GlowCare.Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GlowCare.Entities;

public class GlowCareDbContext 
    : IdentityDbContext<GlowUser>
{
    private GlowCareDbContext()
    {      }

    public GlowCareDbContext(DbContextOptions<GlowCareDbContext> options)
    : base(options)
    {
    }

    public DbSet<Certificate> Certificates {  get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmployeeSchedule> EmployeesSchedules { get; set; }
    public DbSet<GlowUser> GlowUsers { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<Procedure> Procedures { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Service> Service { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .UseSqlServer("Server=DESKTOP-6VQ6QDR\\SQLEXPRESS;Database=GlowCare;Integrated Security=true;TrustServerCertificate=true;");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}