using GymManagementSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(CancellationToken ct = default);

        Task<Session?> GetSessionWithTrainerAndCategoryAsync(int sessionId ,CancellationToken ct = default);

        Task<int> GetCountOfBookedSlotAsync(int sessionId, CancellationToken ct = default);

    }
}
