using GymManagementSystem.Context;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext dbContext;
        public GenericRepository(GymDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        public async Task<IEnumerable<TEntity>> GetAll(bool isTracked, CancellationToken ct = default)
        {
            var entities = isTracked ? dbContext.Set<TEntity>() : dbContext.Set<TEntity>().AsNoTracking();
            return await entities.ToListAsync();
        }

        public async Task<TEntity?> GetById(int id, CancellationToken ct = default)
        {
            var entity = await dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
            return entity;
        }

        public void Add(TEntity entity)
        {
            dbContext.Set<TEntity>().Add(entity);
        }

        public void Update(TEntity entity)
        {
            dbContext.Set<TEntity>().Update(entity);
        }

        public async void Delete(int id)
        {
            var entity = await dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
            if (entity != null)
                dbContext.Set<TEntity>().Remove(entity);
        }

        public async Task<int> CompleteAsync()
        {
            return await dbContext.SaveChangesAsync();
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool isTracked = false, CancellationToken ct = default)
        {
            var entities = isTracked ? dbContext.Set<TEntity>() : dbContext.Set<TEntity>().AsNoTracking();
            return await entities.FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await dbContext.Set<TEntity>().AnyAsync(predicate, ct);
        }
    }
}
