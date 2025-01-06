using System;
using System.Collections.Generic;
using LoginMS.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginMS.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TfaRol> TfaRols { get; set; }

    public virtual DbSet<TfaUser> TfaUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConStr");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Modern_Spanish_CI_AI");

        modelBuilder.Entity<TfaRol>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__TFA_ROL__5402365444BEE72A");

            entity.ToTable("TFA_ROL");

            entity.Property(e => e.RolId).HasColumnName("rolID");
            entity.Property(e => e.RolDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("rolDescription");
            entity.Property(e => e.RolName)
                .HasMaxLength(50)
                .HasColumnName("rolName");
        });

        modelBuilder.Entity<TfaUser>(entity =>
        {
            entity.HasKey(e => e.UsersId).HasName("PK__TFA_USER__788FDD2552307940");

            entity.ToTable("TFA_USERS");

            entity.Property(e => e.UsersId).HasColumnName("usersID");
            entity.Property(e => e.Contrasenia)
                .HasMaxLength(999)
                .IsUnicode(false)
                .HasColumnName("contrasenia");
            entity.Property(e => e.RolId).HasColumnName("rolID");
            entity.Property(e => e.RolIdaddional).HasColumnName("rolIDAddional");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(100)
                .HasColumnName("userEmail");
            entity.Property(e => e.UserLastName)
                .HasMaxLength(50)
                .HasColumnName("userLastName");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("userName");
            entity.Property(e => e.UserPoints).HasColumnName("userPoints");

            entity.HasOne(d => d.Rol).WithMany(p => p.TfaUserRols)
                .HasForeignKey(d => d.RolId)
                .HasConstraintName("FK_userRol");

            entity.HasOne(d => d.RolIdaddionalNavigation).WithMany(p => p.TfaUserRolIdaddionalNavigations)
                .HasForeignKey(d => d.RolIdaddional)
                .HasConstraintName("FK_userRolAddional");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
