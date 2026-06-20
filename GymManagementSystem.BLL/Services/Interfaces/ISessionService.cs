using GymManagementSystem.BLL.Services.Common;
using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        public Task<IEnumerable<SessionViewModel>> GetAllSessionAsync(bool isTracked = false, CancellationToken ct = default);


        Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default);

        Task<SessionViewModel?> GetSessionById(int sessionId, CancellationToken ct = default);

        Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int sessionId,CancellationToken ct = default);


        // Post
        public Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default);
        Task<Result> UpdateSessionAsync(int sessionId, UpdateSessionViewModel model, CancellationToken ct = default);

        Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct = default);
    }
}
