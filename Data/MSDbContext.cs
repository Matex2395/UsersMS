using LoginMS.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginMS.Data
{
    public class MSDbContext : DbContext
    {
        public MSDbContext() { }

        public MSDbContext(
            DbContextOptions<MSDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //"tfa_users" Table
            modelBuilder.Entity<User>(
                entity =>
                {
                    entity.HasKey(e => e.vli_id);

                    entity.Property(e => e.vli_id).HasColumnName("usersID");
                    entity.Property(e => e.vls_name)
                        .HasMaxLength(50)
                        .HasColumnName("userName");
                    entity.Property(e => e.vls_lastname)
                        .HasMaxLength(50)
                        .HasColumnName("userLastName");
                    entity.Property(e => e.vls_email)
                        .HasMaxLength(100)
                        .HasColumnName("userEmail");
                    entity.Property(e => e.vls_password)
                        .HasMaxLength(255) // CONFIRM PASSWORD LENGTH (ENCRYPT)
                        .HasColumnName("userPassword");
                    entity.Property(e => e.vli_points)
                        .HasDefaultValue(0)
                        .HasColumnName("userPoints");
                    entity.Property(e => e.vli_role)
                        .HasColumnName("rolID");
                    entity.Property(e => e.vli_extrarole)
                        .HasColumnName("rolIDAdditional");

                    // Generic Admin user creation
                    entity.HasData(new User
                    {
                        vli_id = 001,
                        vls_name = "Master",
                        vls_lastname = "Admin",
                        vls_email = "master.admin@fisagrp.com",
                        vls_password = "admin123",
                        vli_points = 0,
                        vli_role = 1,
                    });


                    // FK Relations
                    entity.HasOne(u => u.vlo_role) // Main relation (vli_role)
                        .WithMany() // A role can have many users
                        .HasForeignKey(u => u.vli_role) // User foreign key
                        .OnDelete(DeleteBehavior.Restrict); // Deleting configuration
                    entity.HasOne(u => u.vlo_extrarole) // Extra relation (vli_extrarole)
                        .WithMany() // A role can have many users
                        .HasForeignKey(u => u.vli_extrarole) // User foreign key
                        .OnDelete(DeleteBehavior.Restrict); // Deleting configuration
                });

            //"tfa_rol" Table
            modelBuilder.Entity<Role>(
                entity =>
                {
                    entity.HasKey(e => e.vli_id);

                    entity.Property(e => e.vli_id).HasColumnName("rolID");
                    entity.Property(e => e.vls_name)
                        .HasMaxLength(50)
                        .HasColumnName("rolName");
                    entity.Property(e => e.vls_description)
                        .HasMaxLength(50)
                        .HasColumnName("rolDescription");

                    // Role Creation
                    entity.HasData(
                        new Role
                        {
                            vli_id = 1,
                            vls_name = "Administrator",
                            vls_description = "Role that allows access to all system functions.",
                        },
                        new Role
                        {
                            vli_id = 2,
                            vls_name = "Leader",
                            vls_description = "Role that can complete tasks, give diplomas to colaborators and generate reports."
                        },
                        new Role
                        {
                            vli_id = 3,
                            vls_name = "Colaborator",
                            vls_description = "Rol that can complete tasks and receive diplomas"
                        }
                        );
                });
        }
    }
}
