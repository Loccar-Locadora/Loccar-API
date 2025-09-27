using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoccarInfra.ORM.model;

namespace LoccarInfra.Repositories.Interfaces
{
    public interface ILocatarioRepository
    {
        Task<Locatario> ObterLocatarioPorEmail(string email);
        Task<Locatario> CadastrarLocatario(Locatario locatario);
    }

}
