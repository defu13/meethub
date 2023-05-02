using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace mvc_pruebas.Models;

public partial class MeethubdbContext : DbContext
{
    public MeethubdbContext()
    {
    }

    public MeethubdbContext(DbContextOptions<MeethubdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assistant> Assistants { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventsAssistant> EventsAssistants { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=meethubdb;uid=admin;pwd=1234", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.33-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Assistant>(entity =>
        {
            entity.HasKey(e => e.IdAssistant).HasName("PRIMARY");

            entity.ToTable("assistants");

            entity.Property(e => e.IdAssistant).HasColumnName("id_assistant");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(50)
                .HasColumnName("apellidos");
            entity.Property(e => e.Aprobado).HasColumnName("aprobado");
            entity.Property(e => e.Confirmado).HasColumnName("confirmado");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(12)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("PRIMARY");

            entity.ToTable("events");

            entity.HasIndex(e => e.IdUser, "id_user");

            entity.Property(e => e.IdEvent).HasColumnName("id_event");
            entity.Property(e => e.Aforo).HasColumnName("aforo");
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
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Image)
                .HasColumnType("blob")
                .HasColumnName("image");
            entity.Property(e => e.Tipo)
                .HasColumnType("enum('inscripcion','invitacion')")
                .HasColumnName("tipo");
            entity.Property(e => e.Titulo)
                .HasMaxLength(255)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Events)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("events_ibfk_1");
        });

        modelBuilder.Entity<EventsAssistant>(entity =>
        {
            entity.HasKey(e => e.IdEventAssistant).HasName("PRIMARY");

            entity.ToTable("events_assistants");

            entity.HasIndex(e => e.IdAssistant, "id_assistant");

            entity.HasIndex(e => e.IdEvent, "id_event");

            entity.Property(e => e.IdEventAssistant).HasColumnName("id_event_assistant");
            entity.Property(e => e.IdAssistant).HasColumnName("id_assistant");
            entity.Property(e => e.IdEvent).HasColumnName("id_event");

            entity.HasOne(d => d.IdAssistantNavigation).WithMany(p => p.EventsAssistants)
                .HasForeignKey(d => d.IdAssistant)
                .HasConstraintName("events_assistants_ibfk_2");

            entity.HasOne(d => d.IdEventNavigation).WithMany(p => p.EventsAssistants)
                .HasForeignKey(d => d.IdEvent)
                .HasConstraintName("events_assistants_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.IdUser).HasColumnName("id_user");
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
