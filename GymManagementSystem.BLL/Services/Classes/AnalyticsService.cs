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
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<AnalyticsViewModel?> GetAnalyticsAsync(CancellationToken ct = default)
        {
            var membersCount = await _unitOfWork.GetRepository<Member>().CountAsync(ct :ct);
            var trainersCount = await _unitOfWork.GetRepository<Trainer>().CountAsync(ct : ct);
            var activeMemberShips = await _unitOfWork.GetRepository<MemberShip>().CountAsync(ct : ct);

            var upComingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate > DateTime.Now, ct);
            var ongoingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate <= DateTime.Now && s.EndDate > DateTime.Now, ct);
            var completedSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.EndDate < DateTime.Now, ct);

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
