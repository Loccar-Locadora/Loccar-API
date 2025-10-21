using System;
using System.Collections.Generic;

namespace LoccarWebapi.ORM.model;

public partial class Customer
{
    public int Idcustomer { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? DriverLicense { get; set; }

    public DateTime? Created { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
