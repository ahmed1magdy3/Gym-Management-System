using GymManagementSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface IGenericRepository <TEntity> where TEntity : BaseEntity, new()
    {
        Task<IEnumerable<TEntity>> GetAll(bool isTracked, CancellationToken ct = default);
        Task<TEntity?> GetById(int id, CancellationToken ct = default);
        void AddPlan(TEntity entity);
        void Update(TEntity entity);
        void Delete(int id);
        Task<int> CompleteAsync();
    }
}
