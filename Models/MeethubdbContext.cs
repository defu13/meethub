﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace meethub.Models;

public partial class MeethubdbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public MeethubdbContext()
    {
    }

    public MeethubdbContext(IConfiguration configuration, DbContextOptions<MeethubdbContext> options)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<Assistant> Assistants { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<User> Users { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //     => optionsBuilder.UseMySql("server=localhost;port=3306;database=meethubdb;uid=admin;pwd=1234", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.33-mysql"));

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(_configuration.GetConnectionString("Connection"),
            Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.33-mysql"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Assistant>(entity =>
        {
            entity.HasKey(e => e.IdAssistant).HasName("PRIMARY");

            entity.ToTable("assistants");

            entity.HasIndex(e => e.IdEvent, "id_event");

            entity.Property(e => e.IdAssistant)
                .HasColumnType("int(11)")
                .HasColumnName("id_assistant");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(50)
                .HasColumnName("apellidos");
            entity.Property(e => e.Aprobado).HasColumnName("aprobado");
            entity.Property(e => e.Confirmado).HasColumnName("confirmado");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.IdEvent)
                .HasColumnType("int(11)")
                .HasColumnName("id_event");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.QrCode)
                .HasMaxLength(255)
                .HasColumnName("qr_code");
            entity.Property(e => e.Telefono)
                .HasMaxLength(12)
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdEventNavigation).WithMany(p => p.Assistants)
                .HasForeignKey(d => d.IdEvent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("assistants_ibfk_1");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("PRIMARY");

            entity.ToTable("events");

            entity.HasIndex(e => e.IdUser, "id_user");

            entity.Property(e => e.IdEvent)
                .HasColumnType("int(11)")
                .HasColumnName("id_event");
            entity.Property(e => e.Aforo)
                .HasColumnType("int(11)")
                .HasColumnName("aforo");
            entity.Property(e => e.Descripcion)
                .HasColumnType("text")
                .HasColumnName("descripcion");
            entity.Property(e => e.Direccion)
                .HasMaxLength(255)
                .HasColumnName("direccion");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fecha_inicio");
            entity.Property(e => e.IdUser)
                .HasColumnType("int(11)")
                .HasColumnName("id_user");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Tipo)
                .HasMaxLength(25)
                .HasColumnName("tipo");
            entity.Property(e => e.Titulo)
                .HasMaxLength(255)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Events)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("events_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.IdUser)
                .HasColumnType("int(11)")
                .HasColumnName("id_user");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(50)
                .HasColumnName("apellidos");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
