using Bogus;
using LoccarDomain.Customer.Models;
using LoccarDomain.Vehicle.Models;
using LoccarDomain.Reservation.Models;

namespace LoccarTests.Utilities
{
    public static class TestDataGenerator
    {
        private static readonly Faker _faker = new Faker("pt_BR");

        public static Customer GenerateValidCustomer()
        {
            return new Faker<Customer>("pt_BR")
                .RuleFor(c => c.Username, f => f.Name.FullName())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.Cellphone, f => f.Phone.PhoneNumber("11#########"))
                .RuleFor(c => c.DriverLicense, f => f.Random.Number(10000000000L, 99999999999L).ToString())
                .Generate();
        }

        public static List<Customer> GenerateCustomers(int count)
        {
            return new Faker<Customer>("pt_BR")
                .RuleFor(c => c.IdCustomer, f => f.Random.Int(1, 1000))
                .RuleFor(c => c.Username, f => f.Name.FullName())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.Cellphone, f => f.Phone.PhoneNumber("11#########"))
                .RuleFor(c => c.DriverLicense, f => f.Random.Number(10000000000L, 99999999999L).ToString())
                .RuleFor(c => c.Created, f => f.Date.Past())
                .Generate(count);
        }

        public static Vehicle GenerateValidVehicle(VehicleType type = VehicleType.Passenger)
        {
            var vehicle = new Faker<Vehicle>("pt_BR")
                .RuleFor(v => v.Idvehicle, f => f.Random.Int(1, 1000))
                .RuleFor(v => v.Brand, f => f.Vehicle.Manufacturer())
                .RuleFor(v => v.Model, f => f.Vehicle.Model())
                .RuleFor(v => v.ManufacturingYear, f => f.Date.Past(10).Year)
                .RuleFor(v => v.ModelYear, f => f.Date.Past(10).Year)
                .RuleFor(v => v.Vin, f => f.Vehicle.Vin())
                .RuleFor(v => v.DailyRate, f => f.Random.Decimal(50, 500))
                .RuleFor(v => v.MonthlyRate, f => f.Random.Decimal(1000, 10000))
                .RuleFor(v => v.CompanyDailyRate, f => f.Random.Decimal(45, 450))
                .RuleFor(v => v.ReducedDailyRate, f => f.Random.Decimal(40, 400))
                .RuleFor(v => v.FuelTankCapacity, f => f.Random.Int(30, 100))
                .RuleFor(v => v.Reserved, f => f.Random.Bool())
                .RuleFor(v => v.Type, type)
                .Generate();

            // Adicionar especialização baseada no tipo
            switch (type)
            {
                case VehicleType.Cargo:
                    vehicle.CargoVehicle = GenerateCargoVehicle();
                    break;
                case VehicleType.Motorcycle:
                    vehicle.Motorcycle = GenerateMotorcycle();
                    break;
                case VehicleType.Leisure:
                    vehicle.LeisureVehicle = GenerateLeisureVehicle();
                    break;
                case VehicleType.Passenger:
                    vehicle.PassengerVehicle = GeneratePassengerVehicle();
                    break;
            }

            return vehicle;
        }

        public static CargoVehicle GenerateCargoVehicle()
        {
            return new Faker<CargoVehicle>()
                .RuleFor(c => c.CargoCapacity, f => f.Random.Decimal(500, 5000))
                .RuleFor(c => c.CargoType, f => f.PickRandom("Seca", "Refrigerada", "Líquida", "Perigosa"))
                .RuleFor(c => c.TareWeight, f => f.Random.Decimal(1000, 3000))
                .RuleFor(c => c.CargoCompartmentSize, f => f.Random.Decimal(10, 50))
                .Generate();
        }

        public static PassengerVehicle GeneratePassengerVehicle()
        {
            return new Faker<PassengerVehicle>()
                .RuleFor(p => p.PassengerCapacity, f => f.Random.Int(2, 8))
                .RuleFor(p => p.Tv, f => f.Random.Bool())
                .RuleFor(p => p.AirConditioning, f => f.Random.Bool())
                .RuleFor(p => p.PowerSteering, f => f.Random.Bool())
                .Generate();
        }

        public static LeisureVehicle GenerateLeisureVehicle()
        {
            return new Faker<LeisureVehicle>()
                .RuleFor(l => l.Automatic, f => f.Random.Bool())
                .RuleFor(l => l.PowerSteering, f => f.Random.Bool())
                .RuleFor(l => l.AirConditioning, f => f.Random.Bool())
                .RuleFor(l => l.Category, f => f.PickRandom("Econômico", "Executivo", "Luxo", "Esportivo"))
                .Generate();
        }

        public static Motorcycle GenerateMotorcycle()
        {
            return new Faker<Motorcycle>()
                .RuleFor(m => m.TractionControl, f => f.Random.Bool())
                .RuleFor(m => m.AbsBrakes, f => f.Random.Bool())
                .RuleFor(m => m.CruiseControl, f => f.Random.Bool())
                .Generate();
        }

        public static Reservation GenerateValidReservation()
        {
            var startDate = _faker.Date.Future(0, DateTime.Now.AddMonths(1));
            var endDate = startDate.AddDays(_faker.Random.Int(1, 30));

            return new Faker<Reservation>()
                .RuleFor(r => r.Reservationnumber, f => f.Random.Int(100000, 999999))
                .RuleFor(r => r.Idcustomer, f => f.Random.Int(1, 100))
                .RuleFor(r => r.Idvehicle, f => f.Random.Int(1, 100))
                .RuleFor(r => r.RentalDate, startDate)
                .RuleFor(r => r.ReturnDate, endDate)
                .RuleFor(r => r.RentalDays, (endDate - startDate).Days)
                .RuleFor(r => r.DailyRate, f => f.Random.Decimal(50, 300))
                .RuleFor(r => r.RateType, f => f.PickRandom("Diária", "Semanal", "Mensal"))
                .RuleFor(r => r.InsuranceVehicle, f => f.Random.Decimal(10, 100))
                .RuleFor(r => r.InsuranceThirdParty, f => f.Random.Decimal(10, 50))
                .RuleFor(r => r.TaxAmount, f => f.Random.Decimal(5, 30))
                .Generate();
        }

        public static List<Reservation> GenerateReservations(int count)
        {
            return Enumerable.Range(1, count)
                .Select(_ => GenerateValidReservation())
                .ToList();
        }

        // Métodos para cenários específicos de teste
        public static Customer GenerateCustomerWithInvalidEmail()
        {
            var customer = GenerateValidCustomer();
            customer.Email = "invalid-email-format";
            return customer;
        }

        public static Customer GenerateCustomerWithInvalidPhone()
        {
            var customer = GenerateValidCustomer();
            customer.Cellphone = "123"; // Muito curto
            return customer;
        }

        public static Vehicle GenerateVehicleWithNegativeRate()
        {
            var vehicle = GenerateValidVehicle();
            vehicle.DailyRate = -100; // Rate negativa
            return vehicle;
        }

        public static Reservation GenerateReservationWithInvalidDates()
        {
            var reservation = GenerateValidReservation();
            reservation.RentalDate = DateTime.Now.AddDays(10);
            reservation.ReturnDate = DateTime.Now.AddDays(5); // Data de retorno anterior à locação
            return reservation;
        }

        // Método para gerar dados de teste para diferentes culturas/idiomas
        public static Customer GenerateCustomerForCulture(string culture)
        {
            return new Faker<Customer>(culture)
                .RuleFor(c => c.Username, f => f.Name.FullName())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.Cellphone, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.DriverLicense, f => f.Random.AlphaNumeric(10))
                .Generate();
        }
    }
}