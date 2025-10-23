using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LoccarWebapi.ORM.model;

public partial class DataBaseContext : DbContext
{
    public DataBaseContext()
    {
    }

    public DataBaseContext(DbContextOptions<DataBaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CargoVehicle> CargoVehicles { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<LeisureVehicle> LeisureVehicles { get; set; }

    public virtual DbSet<Motorcycle> Motorcycles { get; set; }

    public virtual DbSet<PassengerVehicle> PassengerVehicles { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Server=localhost;Port=5433;Database=loccardb;User Id=postgres;Password=postgres;Pooling=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CargoVehicle>(entity =>
        {
            entity.HasKey(e => e.Idvehicle).HasName("cargo_vehicle_pkey");

            entity.ToTable("cargo_vehicle");

            entity.Property(e => e.Idvehicle)
                .ValueGeneratedNever()
                .HasColumnName("idvehicle");
            entity.Property(e => e.CargoCapacity)
                .HasPrecision(10, 2)
                .HasColumnName("cargo_capacity");
            entity.Property(e => e.CargoCompartmentSize)
                .HasMaxLength(100)
                .HasColumnName("cargo_compartment_size");
            entity.Property(e => e.CargoType)
                .HasMaxLength(50)
                .HasColumnName("cargo_type");
            entity.Property(e => e.TareWeight)
                .HasPrecision(10, 2)
                .HasColumnName("tare_weight");

            entity.HasOne(d => d.IdvehicleNavigation).WithOne(p => p.CargoVehicle)
                .HasForeignKey<CargoVehicle>(d => d.Idvehicle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cargo_vehicle_idvehicle_fkey");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Idcustomer).HasName("customer_pkey");

            entity.ToTable("customer");

            entity.Property(e => e.Idcustomer).HasColumnName("idcustomer");
            entity.Property(e => e.Created)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created");
            entity.Property(e => e.DriverLicense)
                .HasMaxLength(11)
                .HasColumnName("driver_license");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<LeisureVehicle>(entity =>
        {
            entity.HasKey(e => e.Idvehicle).HasName("leisure_vehicle_pkey");

            entity.ToTable("leisure_vehicle");

            entity.Property(e => e.Idvehicle)
                .ValueGeneratedNever()
                .HasColumnName("idvehicle");
            entity.Property(e => e.AirConditioning).HasColumnName("air_conditioning");
            entity.Property(e => e.Automatic).HasColumnName("automatic");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasColumnName("category");
            entity.Property(e => e.PowerSteering).HasColumnName("power_steering");

            entity.HasOne(d => d.IdvehicleNavigation).WithOne(p => p.LeisureVehicle)
                .HasForeignKey<LeisureVehicle>(d => d.Idvehicle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("leisure_vehicle_idvehicle_fkey");
        });

        modelBuilder.Entity<Motorcycle>(entity =>
        {
            entity.HasKey(e => e.Idvehicle).HasName("motorcycle_pkey");

            entity.ToTable("motorcycle");

            entity.Property(e => e.Idvehicle)
                .ValueGeneratedNever()
                .HasColumnName("idvehicle");
            entity.Property(e => e.AbsBrakes).HasColumnName("abs_brakes");
            entity.Property(e => e.CruiseControl).HasColumnName("cruise_control");
            entity.Property(e => e.TractionControl).HasColumnName("traction_control");

            entity.HasOne(d => d.IdvehicleNavigation).WithOne(p => p.Motorcycle)
                .HasForeignKey<Motorcycle>(d => d.Idvehicle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("motorcycle_idvehicle_fkey");
        });

        modelBuilder.Entity<PassengerVehicle>(entity =>
        {
            entity.HasKey(e => e.Idvehicle).HasName("passenger_vehicle_pkey");

            entity.ToTable("passenger_vehicle");

            entity.Property(e => e.Idvehicle)
                .ValueGeneratedNever()
                .HasColumnName("idvehicle");
            entity.Property(e => e.AirConditioning).HasColumnName("air_conditioning");
            entity.Property(e => e.PassengerCapacity).HasColumnName("passenger_capacity");
            entity.Property(e => e.PowerSteering).HasColumnName("power_steering");
            entity.Property(e => e.Tv).HasColumnName("tv");

            entity.HasOne(d => d.IdvehicleNavigation).WithOne(p => p.PassengerVehicle)
                .HasForeignKey<PassengerVehicle>(d => d.Idvehicle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("passenger_vehicle_idvehicle_fkey");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Reservationnumber).HasName("reservation_pkey");

            entity.ToTable("reservation");

            entity.HasIndex(e => e.Idcustomer, "ix_reservation_idcustomer");

            entity.HasIndex(e => e.Idvehicle, "ix_reservation_idvehicle");

            entity.Property(e => e.Reservationnumber).HasColumnName("reservationnumber");
            entity.Property(e => e.DailyRate)
                .HasPrecision(10, 2)
                .HasColumnName("daily_rate");
            entity.Property(e => e.Idcustomer).HasColumnName("idcustomer");
            entity.Property(e => e.Idvehicle).HasColumnName("idvehicle");
            entity.Property(e => e.InsuranceThirdParty)
                .HasPrecision(10, 2)
                .HasColumnName("insurance_third_party");
            entity.Property(e => e.InsuranceVehicle)
                .HasPrecision(10, 2)
                .HasColumnName("insurance_vehicle");
            entity.Property(e => e.RateType)
                .HasMaxLength(20)
                .HasColumnName("rate_type");
            entity.Property(e => e.RentalDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("rental_date");
            entity.Property(e => e.RentalDays).HasColumnName("rental_days");
            entity.Property(e => e.ReturnDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("return_date");
            entity.Property(e => e.TaxAmount)
                .HasPrecision(10, 2)
                .HasColumnName("tax_amount");

            entity.HasOne(d => d.IdcustomerNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.Idcustomer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reservation_idcustomer_fkey");

            entity.HasOne(d => d.IdvehicleNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.Idvehicle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reservation_idvehicle_fkey");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Idvehicle).HasName("vehicle_pkey");

            entity.ToTable("vehicle");

            entity.HasIndex(e => e.Vin, "vehicle_vin_key").IsUnique();

            entity.Property(e => e.Idvehicle).HasColumnName("idvehicle");
            entity.Property(e => e.Available).HasColumnName("available");
            entity.Property(e => e.Brand)
                .HasMaxLength(50)
                .HasColumnName("brand");
            entity.Property(e => e.CompanyDailyRate)
                .HasPrecision(10, 2)
                .HasColumnName("company_daily_rate");
            entity.Property(e => e.DailyRate)
                .HasPrecision(10, 2)
                .HasColumnName("daily_rate");
            entity.Property(e => e.FuelTankCapacity).HasColumnName("fuel_tank_capacity");
            entity.Property(e => e.ManufacturingYear).HasColumnName("manufacturing_year");
            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .HasColumnName("model");
            entity.Property(e => e.ModelYear).HasColumnName("model_year");
            entity.Property(e => e.MonthlyRate)
                .HasPrecision(10, 2)
                .HasColumnName("monthly_rate");
            entity.Property(e => e.ReducedDailyRate)
                .HasPrecision(10, 2)
                .HasColumnName("reduced_daily_rate");
            entity.Property(e => e.Vin)
                .HasMaxLength(11)
                .HasColumnName("vin");
        });
        modelBuilder.HasSequence("customer_idcustomer_seq");
        modelBuilder.HasSequence("locatario_idlocatario_seq");
        modelBuilder.HasSequence("reserva_numeroreserva_seq");
        modelBuilder.HasSequence("reservation_reservationnumber_seq");
        modelBuilder.HasSequence("vehicle_idvehicle_seq");
        modelBuilder.HasSequence("veiculo_idveiculo_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
