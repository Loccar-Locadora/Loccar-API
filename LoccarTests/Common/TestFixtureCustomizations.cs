using AutoFixture;
using AutoFixture.Kernel;
using LoccarDomain.Customer.Models;
using LoccarDomain.Vehicle.Models;
using LoccarDomain.Reservation.Models;
using System.Reflection;

namespace LoccarTests.Common
{
    public class TestFixtureCustomizations : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            // Customizações para Customer
            fixture.Customize<Customer>(composer =>
                composer.With(c => c.Email, "test@example.com")
                       .With(c => c.Cellphone, "11999999999")
                       .With(c => c.DriverLicense, "12345678901")
                       .With(c => c.Username, "TestUser"));

            // Customizações para Vehicle
            fixture.Customize<Vehicle>(composer =>
                composer.With(v => v.Brand, "TestBrand")
                       .With(v => v.Model, "TestModel")
                       .With(v => v.ManufacturingYear, 2020)
                       .With(v => v.ModelYear, 2021)
                       .With(v => v.Vin, "1HGBH41JXMN109186")
                       .With(v => v.DailyRate, 100m)
                       .With(v => v.Reserved, false));

            // Customizações para Reservation
            fixture.Customize<Reservation>(composer =>
                composer.With(r => r.RentalDate, DateTime.Today)
                       .With(r => r.ReturnDate, DateTime.Today.AddDays(7))
                       .With(r => r.DailyRate, 100m)
                       .With(r => r.RentalDays, 7));

            // Customizações para entidades ORM
            fixture.Customize<LoccarInfra.ORM.model.Customer>(composer =>
                composer.With(c => c.Email, "test@example.com")
                       .With(c => c.Phone, "11999999999")
                       .With(c => c.DriverLicense, "12345678901")
                       .With(c => c.Name, "TestUser")
                       .With(c => c.Created, DateTime.Now));

            fixture.Customize<LoccarInfra.ORM.model.Vehicle>(composer =>
                composer.With(v => v.Brand, "TestBrand")
                       .With(v => v.Model, "TestModel")
                       .With(v => v.ManufacturingYear, 2020)
                       .With(v => v.ModelYear, 2021)
                       .With(v => v.Vin, "1HGBH41JXMN109186")
                       .With(v => v.DailyRate, 100m)
                       .With(v => v.Reserved, false));

            // Evitar propriedades de navegação circulares
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}