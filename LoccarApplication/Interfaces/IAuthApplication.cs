using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoccarDomain.Customer;
using LoccarDomain;
using LoccarDomain.LoggedUser.Models;

namespace LoccarApplication.Interfaces
{
    public interface IAuthApplication
    {
        LoggedUser GetLoggedUser();
    }
}
