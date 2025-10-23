using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace LoccarDomain.LoggedUser.Models
{
    public class LoggedUser
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }

        public bool Authenticated { get; set; }
    }
}
