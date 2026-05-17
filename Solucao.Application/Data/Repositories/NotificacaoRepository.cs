using System;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using Solucao.Application.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Solucao.Application.Data.Repositories
{
    public class NotificacaoRepository
    {
        public IUnitOfWork UnitOfWork => Db;
        protected readonly SolucaoContext Db;
        protected readonly DbSet<Notificacao> DbSet;

        public NotificacaoRepository(SolucaoContext _context)
        {
            Db = _context;
            DbSet = Db.Set<Notificacao>();
        }

        // 🔍 Buscar todas
        public async Task<IEnumerable<Notificacao>> GetAll()
        {
            return await DbSet
                .Where(x => x.Active)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // 🔍 Buscar por Id
        public async Task<Notificacao> GetById(Guid id)
        {
            return await DbSet
                .FirstOrDefaultAsync(x => x.Id == id && x.Active);
        }

        // 🔍 Buscar por Token (usado na confirmação)
        public async Task<Notificacao> GetByToken(string token)
        {
            return await DbSet
                .Include(x => x.Locacao)
                .Include(x => x.Locacao.Client)
                .Include(x => x.Locacao.Equipament)
                .FirstOrDefaultAsync(x => x.TokenConfirmacao == token && x.Active);
        }

        // 🔍 Buscar pendentes (para envio ou reenvio)
        public async Task<IEnumerable<Notificacao>> GetPendentes()
        {
            return await DbSet
                .Where(x => x.Status == 'P' && x.Active)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // ➕ Adicionar
        public async Task<ValidationResult> Add(Notificacao notificacao)
        {
            try
            {
                Db.Entry(notificacao).State = EntityState.Added;

                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // ✏️ Atualizar
        public async Task<ValidationResult> Update(Notificacao notificacao)
        {
            try
            {
                Db.Entry(notificacao).State = EntityState.Modified;

                await Db.SaveChangesAsync();
                return ValidationResult.Success;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // 🔄 Atualizar status (uso frequente)
        public async Task AtualizarStatus(Guid id, char status)
        {
            var notificacao = await GetById(id);

            if (notificacao == null) return;

            notificacao.Status = status;
            notificacao.UpdatedAt = DateTime.Now;

            Db.Entry(notificacao).State = EntityState.Modified;

            await Db.SaveChangesAsync();
        }

        // ✅ Registrar resposta
        public async Task RegistrarResposta(string token, string resposta)
        {
            var notificacao = await GetByToken(token);

            if (notificacao == null) return;

            notificacao.Status = 'R';
            notificacao.Resposta = resposta;
            notificacao.DataResposta = DateTime.Now;
            notificacao.UpdatedAt = DateTime.Now;

            Db.Entry(notificacao).State = EntityState.Modified;

            await Db.SaveChangesAsync();
        }

        public async Task<bool> ExistePorLocacao(Guid locacaoId)
        {
            return await DbSet.AnyAsync(x => x.LocacaoId == locacaoId && x.Active);
        }
    }
}