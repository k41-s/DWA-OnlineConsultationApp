using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models;

public partial class ConsultationsContext : DbContext
{
    public ConsultationsContext()
    {
    }

    public ConsultationsContext(DbContextOptions<ConsultationsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<Consultation> Consultations { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Mentor> Mentors { get; set; }

    public virtual DbSet<TypeOfWork> TypeOfWorks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Area__3214EC07AF92FA19");

            entity.ToTable("Area");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Consultation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Consulta__3214EC07357FCDA4");

            entity.ToTable("Consultation");

            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.RequestedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Mentor).WithMany(p => p.Consultations)
                .HasForeignKey(d => d.MentorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Consultat__Mento__35BCFE0A");

            entity.HasOne(d => d.User).WithMany(p => p.Consultations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Consultat__UserI__36B12243");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Log__3214EC0772F5C232");

            entity.ToTable("Log");

            entity.Property(e => e.Level).HasMaxLength(50);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<Mentor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Mentor__3214EC0761E84797");

            entity.ToTable("Mentor");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithOne(p => p.Mentor)
                .HasForeignKey<Mentor>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Mentor__Id__2C3393D0");

            entity.HasOne(d => d.TypeOfWork).WithMany(p => p.Mentors)
                .HasForeignKey(d => d.TypeOfWorkId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Mentor__TypeOfWo__2D27B809");

            entity.HasMany(d => d.Areas).WithMany(p => p.Mentors)
                .UsingEntity<Dictionary<string, object>>(
                    "MentorArea",
                    r => r.HasOne<Area>().WithMany()
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MentorAre__AreaI__30F848ED"),
                    l => l.HasOne<Mentor>().WithMany()
                        .HasForeignKey("MentorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MentorAre__Mento__300424B4"),
                    j =>
                    {
                        j.HasKey("MentorId", "AreaId").HasName("PK__MentorAr__9230FC9C61A65B30");
                        j.ToTable("MentorArea");
                    });
        });

        modelBuilder.Entity<TypeOfWork>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TypeOfWo__3214EC073C51525F");

            entity.ToTable("TypeOfWork");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07D87014B1");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534794E9966").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
