using GymManagementBLL.ViewModels.AnalyticsViewModels;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IGenericRepository<Member> _memberRepo;
        private readonly IGenericRepository<Trainer> _trainerRepo;
        private readonly IGenericRepository<Session> _sessionRepo;
        private readonly IGenericRepository<MemberShip> _memberShipRepo;

        public AnalyticsService(IGenericRepository<Member> memberRepo,
                                IGenericRepository<Trainer>trainerRepo,
                                IGenericRepository<Session>sessionRepo,
                                IGenericRepository<MemberShip>memberShipRepo)
        {
            _memberRepo = memberRepo;
            _trainerRepo = trainerRepo;
            _sessionRepo = sessionRepo;
            _memberShipRepo = memberShipRepo;
        }
        public async Task<AnalyticsViewModel?> GetAnalyticsAsync(CancellationToken ct = default)
        {
            var membersCount = await _memberRepo.CountAsync(ct :ct);
            var trainersCount = await _trainerRepo.CountAsync(ct : ct);
            var activeMemberShips = await _memberShipRepo.CountAsync(ct : ct);

            var upComingSessions = await _sessionRepo.CountAsync(s => s.StartDate > DateTime.Now, ct);
            var ongoingSessions = await _sessionRepo.CountAsync(s => s.StartDate <= DateTime.Now && s.EndDate > DateTime.Now, ct);
            var completedSessions = await _sessionRepo.CountAsync(s => s.EndDate < DateTime.Now, ct);

            var analyticsViewModel = new AnalyticsViewModel()
            {
                TotalMembers = membersCount,
                TotalTrainers = trainersCount,
                ActiveMembers = activeMemberShips,
                UpcomingSessions = upComingSessions,
                OngoingSessions = ongoingSessions,
                CompletedSessions = completedSessions
            };

            return analyticsViewModel;
        }
    }
}
