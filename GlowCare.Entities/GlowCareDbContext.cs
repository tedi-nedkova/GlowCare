using GlowCare.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GlowCare.Entities;

public class GlowCareDbContext
    : IdentityDbContext<GlowUser, IdentityRole<Guid>, Guid>
{
    public GlowCareDbContext(DbContextOptions<GlowCareDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<EmployeeService> EmployeesServices { get; set; } = null!;
    public DbSet<Membership> Memberships { get; set; } = null!;
    public DbSet<Procedure> Procedures { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;
    public DbSet<Schedule> Schedules { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<Review>()
            .HasOne(r => r.Employee)
            .WithMany(e => e.Reviews)
            .HasForeignKey(r => r.EmployeeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Review>()
            .HasOne(r => r.Procedure)
            .WithMany(p => p.Reviews)
            .HasForeignKey(sd => sd.ProcedureId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Review>()
            .HasOne(r => r.Service)
            .WithMany()
            .HasForeignKey(r => r.ServiceId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Procedure>()
            .HasOne(p => p.Employee)
            .WithMany(e => e.Procedures)
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}