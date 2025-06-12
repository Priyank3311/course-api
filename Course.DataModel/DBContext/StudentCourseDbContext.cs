using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Course.DataModel.Entities;

public partial class StudentCourseDbContext : DbContext
{
    public StudentCourseDbContext(DbContextOptions<StudentCourseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("courses_pkey");

            entity.ToTable("courses");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Coursename)
                .HasMaxLength(200)
                .HasColumnName("coursename");
            entity.Property(e => e.Credits).HasColumnName("credits");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .HasColumnName("department");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("enrollments_pkey");

            entity.ToTable("enrollments");

            entity.HasIndex(e => new { e.Userid, e.Courseid }, "unique_enrollment").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Courseid).HasColumnName("courseid");
            entity.Property(e => e.Iscompleted)
                .HasDefaultValue(false)
                .HasColumnName("iscompleted");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.Courseid)
                .HasConstraintName("fk_course");

            entity.HasOne(d => d.User).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("fk_user");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Username, "users_username_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Passwordhash).HasColumnName("passwordhash");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
