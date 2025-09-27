using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LoccarInfra.ORM.model;

public partial class DataBaseContext : DbContext
{
    public DataBaseContext()
    {
    }

    public DataBaseContext(DbContextOptions<DataBaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Locatario> Locatarios { get; set; }

    public virtual DbSet<Motocicletum> Motocicleta { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Veiculo> Veiculos { get; set; }

    public virtual DbSet<VeiculoCarga> VeiculoCargas { get; set; }

    public virtual DbSet<VeiculoPassageiro> VeiculoPassageiros { get; set; }

    public virtual DbSet<VeiculoPasseio> VeiculoPasseios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Locatario>(entity =>
        {
            entity.HasKey(e => e.Idlocatario).HasName("locatario_pkey");

            entity.ToTable("locatario");

            entity.Property(e => e.Idlocatario).HasColumnName("idlocatario");
            entity.Property(e => e.Cnh)
                .HasMaxLength(11)
                .HasColumnName("cnh");
            entity.Property(e => e.Created)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");
            entity.Property(e => e.Telefone)
                .HasMaxLength(50)
                .HasColumnName("telefone");
        });

        modelBuilder.Entity<Motocicletum>(entity =>
        {
            entity.HasKey(e => e.Idveiculo).HasName("motocicleta_pkey");

            entity.ToTable("motocicleta");

            entity.Property(e => e.Idveiculo)
                .ValueGeneratedNever()
                .HasColumnName("idveiculo");
            entity.Property(e => e.Controletracao).HasColumnName("controletracao");
            entity.Property(e => e.Freioabs).HasColumnName("freioabs");
            entity.Property(e => e.Pilotoautomatico).HasColumnName("pilotoautomatico");

            entity.HasOne(d => d.IdveiculoNavigation).WithOne(p => p.Motocicletum)
                .HasForeignKey<Motocicletum>(d => d.Idveiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("motocicleta_idveiculo_fkey");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.Numeroreserva).HasName("reserva_pkey");

            entity.ToTable("reserva");

            entity.HasIndex(e => e.Idlocatario, "ix_reserva_idlocatario");

            entity.HasIndex(e => e.Idveiculo, "ix_reserva_idveiculo");

            entity.Property(e => e.Numeroreserva).HasColumnName("numeroreserva");
            entity.Property(e => e.Dataentrega)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("dataentrega");
            entity.Property(e => e.Datalocacao)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("datalocacao");
            entity.Property(e => e.Diarias).HasColumnName("diarias");
            entity.Property(e => e.Idlocatario).HasColumnName("idlocatario");
            entity.Property(e => e.Idveiculo).HasColumnName("idveiculo");
            entity.Property(e => e.Tipodiaria)
                .HasMaxLength(20)
                .HasColumnName("tipodiaria");
            entity.Property(e => e.Valordiaria)
                .HasPrecision(10, 2)
                .HasColumnName("valordiaria");
            entity.Property(e => e.Valorimposto)
                .HasPrecision(10, 2)
                .HasColumnName("valorimposto");
            entity.Property(e => e.Valorsegurocarro)
                .HasPrecision(10, 2)
                .HasColumnName("valorsegurocarro");
            entity.Property(e => e.Valorseguroterceiro)
                .HasPrecision(10, 2)
                .HasColumnName("valorseguroterceiro");

            entity.HasOne(d => d.IdlocatarioNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.Idlocatario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reserva_idlocatario_fkey");

            entity.HasOne(d => d.IdveiculoNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.Idveiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reserva_idveiculo_fkey");
        });

        modelBuilder.Entity<Veiculo>(entity =>
        {
            entity.HasKey(e => e.Idveiculo).HasName("veiculo_pkey");

            entity.ToTable("veiculo");

            entity.HasIndex(e => e.Renavam, "veiculo_renavam_key").IsUnique();

            entity.Property(e => e.Idveiculo).HasColumnName("idveiculo");
            entity.Property(e => e.Anofabricacao).HasColumnName("anofabricacao");
            entity.Property(e => e.Anomodelo).HasColumnName("anomodelo");
            entity.Property(e => e.Capacidadetanque).HasColumnName("capacidadetanque");
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .HasColumnName("marca");
            entity.Property(e => e.Modelo)
                .HasMaxLength(50)
                .HasColumnName("modelo");
            entity.Property(e => e.Renavam)
                .HasMaxLength(11)
                .HasColumnName("renavam");
            entity.Property(e => e.Valordiaria)
                .HasPrecision(10, 2)
                .HasColumnName("valordiaria");
            entity.Property(e => e.Valordiariaempresa)
                .HasPrecision(10, 2)
                .HasColumnName("valordiariaempresa");
            entity.Property(e => e.Valordiariamensal)
                .HasPrecision(10, 2)
                .HasColumnName("valordiariamensal");
            entity.Property(e => e.Valordiariareduzida)
                .HasPrecision(10, 2)
                .HasColumnName("valordiariareduzida");
        });

        modelBuilder.Entity<VeiculoCarga>(entity =>
        {
            entity.HasKey(e => e.Idveiculo).HasName("veiculo_carga_pkey");

            entity.ToTable("veiculo_carga");

            entity.Property(e => e.Idveiculo)
                .ValueGeneratedNever()
                .HasColumnName("idveiculo");
            entity.Property(e => e.Capacidadecarga)
                .HasPrecision(10, 2)
                .HasColumnName("capacidadecarga");
            entity.Property(e => e.Tamanhocompartimentocarga)
                .HasMaxLength(100)
                .HasColumnName("tamanhocompartimentocarga");
            entity.Property(e => e.Tara)
                .HasPrecision(10, 2)
                .HasColumnName("tara");
            entity.Property(e => e.Tipocarga)
                .HasMaxLength(50)
                .HasColumnName("tipocarga");

            entity.HasOne(d => d.IdveiculoNavigation).WithOne(p => p.VeiculoCarga)
                .HasForeignKey<VeiculoCarga>(d => d.Idveiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("veiculo_carga_idveiculo_fkey");
        });

        modelBuilder.Entity<VeiculoPassageiro>(entity =>
        {
            entity.HasKey(e => e.Idveiculo).HasName("veiculo_passageiro_pkey");

            entity.ToTable("veiculo_passageiro");

            entity.Property(e => e.Idveiculo)
                .ValueGeneratedNever()
                .HasColumnName("idveiculo");
            entity.Property(e => e.Arcondicionado).HasColumnName("arcondicionado");
            entity.Property(e => e.Capacidadepassageiro).HasColumnName("capacidadepassageiro");
            entity.Property(e => e.Direcaohidraulica).HasColumnName("direcaohidraulica");
            entity.Property(e => e.Televisao).HasColumnName("televisao");

            entity.HasOne(d => d.IdveiculoNavigation).WithOne(p => p.VeiculoPassageiro)
                .HasForeignKey<VeiculoPassageiro>(d => d.Idveiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("veiculo_passageiro_idveiculo_fkey");
        });

        modelBuilder.Entity<VeiculoPasseio>(entity =>
        {
            entity.HasKey(e => e.Idveiculo).HasName("veiculo_passeio_pkey");

            entity.ToTable("veiculo_passeio");

            entity.Property(e => e.Idveiculo)
                .ValueGeneratedNever()
                .HasColumnName("idveiculo");
            entity.Property(e => e.Arcondicionado).HasColumnName("arcondicionado");
            entity.Property(e => e.Automatico).HasColumnName("automatico");
            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .HasColumnName("categoria");
            entity.Property(e => e.Direcaohidraulica).HasColumnName("direcaohidraulica");

            entity.HasOne(d => d.IdveiculoNavigation).WithOne(p => p.VeiculoPasseio)
                .HasForeignKey<VeiculoPasseio>(d => d.Idveiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("veiculo_passeio_idveiculo_fkey");
        });
        modelBuilder.HasSequence("locatario_idlocatario_seq");
        modelBuilder.HasSequence("reserva_numeroreserva_seq");
        modelBuilder.HasSequence("veiculo_idveiculo_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
