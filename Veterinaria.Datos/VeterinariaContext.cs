using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Veterinaria.Entidades.Models;

namespace Veterinaria.Datos;

public partial class VeterinariaContext : DbContext
{
    public VeterinariaContext(DbContextOptions<VeterinariaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Dueno> Duenos { get; set; }

    public virtual DbSet<Mascota> Mascotas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dueno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Dueno__3214EC073E02DA0D");

            entity.ToTable("Dueno");

            entity.HasIndex(e => e.Dni, "UQ__Dueno__C0308575647F3C5F").IsUnique();

            entity.Property(e => e.Dni).HasMaxLength(8);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(15);
        });

        modelBuilder.Entity<Mascota>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Mascota__3214EC07BE2C5D0C");
            entity.ToTable("Mascota");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Especie).HasMaxLength(30);
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Peso).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Raza).HasMaxLength(50);

            entity.HasOne(d => d.Dueno).WithMany(p => p.Mascotas)
                .HasForeignKey(d => d.DuenoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mascota_Dueno");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasIndex(e => e.Correo).IsUnique();

            entity.Property(e => e.ClaveHash).HasMaxLength(100);
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Rol).HasMaxLength(30);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
