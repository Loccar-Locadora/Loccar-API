using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoccarInfra.ORM.model;
using LoccarInfra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LoccarInfra.Repositories
{
    public class LocatarioRepository : ILocatarioRepository
    {
        readonly DataBaseContext _dbContext;
        public LocatarioRepository(DataBaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Locatario> CadastrarLocatario(Locatario locatario)
        {
            await _dbContext.Locatarios.AddAsync(locatario);
            await _dbContext.SaveChangesAsync();

            return locatario;
        }

        public async Task<Locatario> ObterLocatarioPorEmail(string email)
        {
            return await _dbContext.Locatarios.Where(n => n.Email.Equals(email)).FirstOrDefaultAsync();
        }
    }
}
