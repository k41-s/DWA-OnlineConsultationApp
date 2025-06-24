using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Models;

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
            entity.HasKey(e => e.Id).HasName("PK__Area__3214EC0766C043CC");

            entity.ToTable("Area");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Consultation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Consulta__3214EC07725E0ECE");

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
                .HasConstraintName("FK__Consultat__Mento__34C8D9D1");

            entity.HasOne(d => d.User).WithMany(p => p.Consultations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Consultat__UserI__35BCFE0A");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Log__3214EC0713E27E42");

            entity.ToTable("Log");

            entity.Property(e => e.Level).HasMaxLength(50);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<Mentor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Mentor__3214EC0719F211E3");

            entity.ToTable("Mentor");

            entity.Property(e => e.ImagePath).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Surname).HasMaxLength(100);

            entity.HasOne(d => d.TypeOfWork).WithMany(p => p.Mentors)
                .HasForeignKey(d => d.TypeOfWorkId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Mentor__TypeOfWo__2C3393D0");

            entity.HasMany(d => d.Areas).WithMany(p => p.Mentors)
                .UsingEntity<Dictionary<string, object>>(
                    "MentorArea",
                    r => r.HasOne<Area>().WithMany()
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MentorAre__AreaI__300424B4"),
                    l => l.HasOne<Mentor>().WithMany()
                        .HasForeignKey("MentorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MentorAre__Mento__2F10007B"),
                    j =>
                    {
                        j.HasKey("MentorId", "AreaId").HasName("PK__MentorAr__9230FC9CC0B3315A");
                        j.ToTable("MentorArea");
                    });
        });

        modelBuilder.Entity<TypeOfWork>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TypeOfWo__3214EC07CE0F317A");

            entity.ToTable("TypeOfWork");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07AC995103");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534539C007B").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(10);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
