using System.Security.Cryptography;
using System.Text;
using LoccarApplication.Interfaces;
using LoccarDomain;
using LoccarDomain.Locatario.Models;
using LoccarInfra.Repositories;
using LoccarInfra.Repositories.Interfaces;

namespace LoccarApplication
{
    public class LocatarioApplication : ILocatarioApplication
    {
        readonly ILocatarioRepository _locatarioRepository;
        public LocatarioApplication(ILocatarioRepository locatarioRepository)
        {
            _locatarioRepository = locatarioRepository;
        }
        public async Task<BaseReturn<Locatario>> RegisterLocatario(Locatario locatario)
        {
            BaseReturn<Locatario> baseReturn = new BaseReturn<Locatario>();

            try
            {
                LoccarInfra.ORM.model.Locatario tabelaLocatario = new LoccarInfra.ORM.model.Locatario()
                {
                    Nome = locatario.Username,
                    Email = locatario.Email,
                    Telefone = locatario.Cellphone,
                    Cnh = locatario.Cnh,
                    Created = DateTime.Now
                };

                var response = await _locatarioRepository.CadastrarLocatario(tabelaLocatario);

                Locatario locatarioResponse = new Locatario()
                {
                    Username = response.Nome,
                    Email = response.Email,
                    Cellphone = response.Telefone,
                    Cnh = response.Cnh,
                    Created = response.Created
                };

                baseReturn.Code = "200";
                baseReturn.Data = locatarioResponse;
                baseReturn.Message = "Locatário cadastrado com sucesso.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = ex.Message;
            }

            return baseReturn;
        }
    }
}
