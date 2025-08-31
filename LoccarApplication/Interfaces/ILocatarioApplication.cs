using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoccarDomain.Locatario;
using LoccarDomain;
using LoccarDomain.Locatario.Models;

namespace LoccarApplication.Interfaces
{
    public interface ILocatarioApplication
    {
        BaseReturn<Locatario> CadastrarLocatario(Locatario locatario); 
    }
}
