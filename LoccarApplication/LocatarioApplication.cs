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
        public BaseReturn<Locatario> CadastrarLocatario(Locatario locatario)
        {
            BaseReturn<Locatario> baseReturn = new BaseReturn<Locatario>();

            try
            {
                var locatarioExistente = _locatarioRepository.ObterLocatarioPorEmail(locatario.Email);
                if (locatarioExistente != null)
                {
                    baseReturn.Code = "400";
                    baseReturn.Message = "E-mail já cadastrado.";
                    return baseReturn;
                }

                string senhaHash = HashSenha(locatario.Senha);

                LoccarInfra.ORM.model.Locatario tabelaLocatario = new LoccarInfra.ORM.model.Locatario()
                {
                    Nome = locatario.Nome,
                    Email = locatario.Email,
                    Telefone = locatario.Telefone,
                    Locador = locatario.Locador,
                    Login = locatario.Login, // usuário de login
                    Senha = senhaHash    // senha armazenada em hash
                };

                var tbLocatario = _locatarioRepository.CadastrarLocatario(tabelaLocatario);

                if(locatario.PessoaFisica != null)
                {
                    LoccarInfra.ORM.model.PessoaFisica pessoaFisica = new LoccarInfra.ORM.model.PessoaFisica()
                    {
                        Idlocatario = tbLocatario.Idlocatario,
                        Cpf = locatario.PessoaFisica.Cpf,
                        EstadoCivil = locatario.PessoaFisica.EstadoCivil
                        
                    };
                    _locatarioRepository.CadastrarPessoaFisica(pessoaFisica);
                }
                else if(locatario.PessoaJuridica != null)
                {
                    LoccarInfra.ORM.model.PessoaJuridica pessoaJuridica = new LoccarInfra.ORM.model.PessoaJuridica()
                    {
                        Idlocatario = tbLocatario.Idlocatario,
                        Cnpj = locatario.PessoaJuridica.Cnpj
                    };
                    _locatarioRepository.CadastrarPessoaJuridica(pessoaJuridica);
                }

                baseReturn.Code = "200";
                baseReturn.Data = locatario;
                baseReturn.Message = "Locatário cadastrado com sucesso.";
            }
            catch (Exception ex)
            {
                baseReturn.Code = "500";
                baseReturn.Message = ex.Message;
            }

            return baseReturn;
        }

        // Função auxiliar para gerar hash seguro da senha
        private string HashSenha(string senha)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

}
}
