using GlowCare.Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GlowCare.Entities;

public class GlowCareDbContext
    : IdentityDbContext<GlowUser>
{
    private GlowCareDbContext()
    { }

    public GlowCareDbContext(DbContextOptions<GlowCareDbContext> options)
    : base(options)
    { }

    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmployeeSchedule> EmployeesSchedules { get; set; }
    public DbSet<EmployeeService> EmployeesServices { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<Procedure> Procedures { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<ScheduleDayOfWeek> SchedulesDaysOfWeek { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Models.DayOfWeek> DaysOfWeek { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      
    }
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

        builder.Entity<Procedure>()
             .HasOne(p => p.Employee)
             .WithMany(e => e.Procedures)
             .HasForeignKey(p => p.EmployeeId)
             .OnDelete(DeleteBehavior.NoAction);
    }
}