using Microsoft.EntityFrameworkCore;
using MyWebApp.Models;
namespace MyWebApp.Data;
public class SchoolContext(DbContextOptions<SchoolContext> options) : DbContext(options)
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Learner> Learners => Set<Learner>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Major> Majors => Set<Major>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Major>().ToTable("Major");
        modelBuilder.Entity<Course>().ToTable("Course");
        modelBuilder.Entity<Learner>().ToTable("Learner");
        modelBuilder.Entity<Enrollment>().ToTable("Enrollment", t =>
            t.HasCheckConstraint("CK_Enrollment_Grade", "[Grade] >= 0 AND [Grade] <= 10"));
        modelBuilder.Entity<Learner>().HasOne(l => l.Major).WithMany(m => m.Learners)
            .HasForeignKey(l => l.MajorID).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrollment>().HasOne(e => e.Learner).WithMany(l => l.Enrollments)
            .HasForeignKey(e => e.LearnerID).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrollment>().HasOne(e => e.Course).WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseID).OnDelete(DeleteBehavior.Restrict);
    }
}
