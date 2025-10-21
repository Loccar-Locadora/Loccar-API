using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoccarDomain;
using LoccarDomain.Vehicle.Models;

namespace LoccarApplication.Interfaces
{
    public interface IVehicleApplication
    {
        Task<BaseReturn<Vehicle>> RegisterVehicle(Vehicle vehicle);
        Task<BaseReturn<List<Vehicle>>> ListAvailableVehicles();
        Task<BaseReturn<bool>> SetVehicleMaintenance(int vehicleId, bool inMaintenance);

    }
}
