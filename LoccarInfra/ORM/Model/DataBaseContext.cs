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

    public virtual DbSet<CargoVehicle> CargoVehicles { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<LeisureVehicle> LeisureVehicles { get; set; }

    public virtual DbSet<Motorcycle> Motorcycles { get; set; }

    public virtual DbSet<PassengerVehicle> PassengerVehicles { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CargoVehicle>(entity =>
        {
            entity.HasKey(e => e.IdVehicle).HasName("cargo_vehicle_pkey");

            entity.ToTable("cargo_vehicle");

            entity.Property(e => e.IdVehicle)
                .ValueGeneratedNever()
                .HasColumnName("id_vehicle");
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

            entity.HasOne(d => d.IdVehicleNavigation).WithOne(p => p.CargoVehicle)
                .HasForeignKey<CargoVehicle>(d => d.IdVehicle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cargo_vehicle_id_vehicle_fkey");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.IdCustomer).HasName("customer_pkey");

            entity.ToTable("customer");

            entity.Property(e => e.IdCustomer).HasColumnName("id_customer");
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
            entity.HasKey(e => e.IdVehicle).HasName("leisure_vehicle_pkey");

            entity.ToTable("leisure_vehicle");

            entity.Property(e => e.IdVehicle)
                .ValueGeneratedNever()
                .HasColumnName("id_vehicle");
            entity.Property(e => e.AirConditioning).HasColumnName("air_conditioning");
            entity.Property(e => e.Automatic).HasColumnName("automatic");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasColumnName("category");
            entity.Property(e => e.PowerSteering).HasColumnName("power_steering");

            entity.HasOne(d => d.IdVehicleNavigation).WithOne(p => p.LeisureVehicle)
                .HasForeignKey<LeisureVehicle>(d => d.IdVehicle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("leisure_vehicle_id_vehicle_fkey");
        });

        modelBuilder.Entity<Motorcycle>(entity =>
        {
            entity.HasKey(e => e.IdVehicle).HasName("motorcycle_pkey");

            entity.ToTable("motorcycle");

            entity.Property(e => e.IdVehicle)
                .ValueGeneratedNever()
                .HasColumnName("id_vehicle");
            entity.Property(e => e.AbsBrakes).HasColumnName("abs_brakes");
            entity.Property(e => e.CruiseControl).HasColumnName("cruise_control");
            entity.Property(e => e.TractionControl).HasColumnName("traction_control");

            entity.HasOne(d => d.IdVehicleNavigation).WithOne(p => p.Motorcycle)
                .HasForeignKey<Motorcycle>(d => d.IdVehicle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("motorcycle_id_vehicle_fkey");
        });

        modelBuilder.Entity<PassengerVehicle>(entity =>
        {
            entity.HasKey(e => e.IdVehicle).HasName("passenger_vehicle_pkey");

            entity.ToTable("passenger_vehicle");

            entity.Property(e => e.IdVehicle)
                .ValueGeneratedNever()
                .HasColumnName("id_vehicle");
            entity.Property(e => e.AirConditioning).HasColumnName("air_conditioning");
            entity.Property(e => e.PassengerCapacity).HasColumnName("passenger_capacity");
            entity.Property(e => e.PowerSteering).HasColumnName("power_steering");
            entity.Property(e => e.Tv).HasColumnName("tv");

            entity.HasOne(d => d.IdVehicleNavigation).WithOne(p => p.PassengerVehicle)
                .HasForeignKey<PassengerVehicle>(d => d.IdVehicle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("passenger_vehicle_id_vehicle_fkey");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Reservationnumber).HasName("reservation_pkey");

            entity.ToTable("reservation");

            entity.HasIndex(e => e.IdCustomer, "ix_reservation_id_customer");

            entity.HasIndex(e => e.IdVehicle, "ix_reservation_id_vehicle");

            entity.Property(e => e.Reservationnumber).HasColumnName("reservationnumber");
            entity.Property(e => e.DailyRate)
                .HasPrecision(10, 2)
                .HasColumnName("daily_rate");
            entity.Property(e => e.IdCustomer).HasColumnName("id_customer");
            entity.Property(e => e.IdVehicle).HasColumnName("id_vehicle");
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
            entity.Property(e => e.DamageDescription)
                .HasColumnName("damage_description");
            entity.HasOne(d => d.IdCustomerNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.IdCustomer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reservation_id_customer_fkey");

            entity.HasOne(d => d.IdVehicleNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.IdVehicle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reservation_id_vehicle_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("user_roles_role_id_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("user_roles_user_id_fkey"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("user_roles_pkey");
                        j.ToTable("user_roles");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                    });
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.IdVehicle).HasName("vehicle_pkey");

            entity.ToTable("vehicle");

            entity.HasIndex(e => e.Vin, "vehicle_vin_key").IsUnique();

            entity.Property(e => e.IdVehicle).HasColumnName("id_vehicle");
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
            entity.Property(e => e.Reserved).HasColumnName("reserved");
            entity.Property(e => e.Vin)
                .HasMaxLength(11)
                .HasColumnName("vin");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
