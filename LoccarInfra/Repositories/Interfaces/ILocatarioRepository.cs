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
        public Locatario CadastrarLocatario(Locatario locatario);
        public Locatario ObterLocatarioPorEmail(string email);
        public PessoaFisica CadastrarPessoaFisica(PessoaFisica pessoaFisica);
        public PessoaJuridica CadastrarPessoaJuridica(PessoaJuridica pessoaJuridica);
    }
}
