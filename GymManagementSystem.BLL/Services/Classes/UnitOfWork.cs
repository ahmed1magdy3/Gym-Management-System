using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.Context;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext dbContext;
        private readonly Dictionary<string, object> _Repos = [];

        public ISessionRepository sessionRepository { get;}

        public UnitOfWork(GymDbContext dbContext)
        {
            this.dbContext = dbContext;
            sessionRepository = new SessionRepository(dbContext);
        }
        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var typeName = typeof(TEntity).Name;

            if (_Repos.TryGetValue(typeName, out object oldRepo))
                return (IGenericRepository<TEntity>) oldRepo;

            var newRepo = new GenericRepository<TEntity>(dbContext);
            _Repos.Add(typeName, newRepo);

            return newRepo;

        }

        public async Task<int> CompleteAsync(CancellationToken ct = default)
        {
            return await dbContext.SaveChangesAsync(ct);
        }
    }
}
