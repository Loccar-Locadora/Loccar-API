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

        public Locatario CadastrarLocatario(Locatario locatario)
        {
            var trackedEntity = _dbContext.ChangeTracker.Entries<Locatario>().FirstOrDefault(e => e.Entity.Idlocatario == locatario.Idlocatario);

            if (trackedEntity != null)
            {
                _dbContext.Entry(trackedEntity.Entity).State = EntityState.Detached;
            }

            _dbContext.Locatarios.Add(locatario);
            _dbContext.SaveChanges();

            return locatario;
        }

        public Locatario ObterLocatarioPorEmail(string email)
        {
            return _dbContext.Locatarios.Where(n => n.Email.Equals(email)).FirstOrDefault();
        }

        public PessoaFisica CadastrarPessoaFisica(PessoaFisica pessoaFisica)
        {
            var trackedEntity = _dbContext.ChangeTracker.Entries<PessoaFisica>().FirstOrDefault(e => e.Entity.Idlocatario == pessoaFisica.Idlocatario);

            if (trackedEntity != null)
            {
                _dbContext.Entry(trackedEntity.Entity).State = EntityState.Detached;
            }

            _dbContext.PessoaFisicas.Add(pessoaFisica);
            _dbContext.SaveChanges();

            return pessoaFisica;
        }

        public PessoaJuridica CadastrarPessoaJuridica(PessoaJuridica pessoaJuridica)
        {
            var trackedEntity = _dbContext.ChangeTracker.Entries<PessoaJuridica>().FirstOrDefault(e => e.Entity.Idlocatario == pessoaJuridica.Idlocatario);

            if (trackedEntity != null)
            {
                _dbContext.Entry(trackedEntity.Entity).State = EntityState.Detached;
            }

            _dbContext.PessoaJuridicas.Add(pessoaJuridica);
            _dbContext.SaveChanges();

            return pessoaJuridica;
        }
    }
}
