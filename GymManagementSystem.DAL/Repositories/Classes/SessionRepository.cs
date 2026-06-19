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
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _dbContext;

        public SessionRepository(GymDbContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(CancellationToken ct = default)
        {
            var sessions = _dbContext.Session.AsNoTracking().Include(s=>s.Trainer).Include(s=>s.Category);
            if (sessions.Any())
                return await sessions.ToArrayAsync();

            return [];
        }

        public async Task<Session?> GetSessionWithTrainerAndCategoryAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _dbContext.Session.Include(s => s.Trainer).Include(s => s.Category).
                FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session == null) return null;

            return session;
        }
        public async Task<int> GetCountOfBookedSlotAsync(int sessionId, CancellationToken ct = default)
        {
            return await _dbContext.Booking.AsNoTracking().CountAsync(b => b.SessionId == sessionId,ct);
        }

    }
}
