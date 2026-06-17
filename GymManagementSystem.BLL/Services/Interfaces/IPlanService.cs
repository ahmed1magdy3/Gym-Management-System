using GymManagementSystem.BLL.ViewModels.PlanViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        // Get
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
        Task<PlanViewModel?> GetPlanDetailsAsync(int planId, CancellationToken ct = default);
        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default);


        // Post
        Task<bool> UpdatePlanDetailsAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default);
        Task<bool> ActivationPlanAsync(int planId, CancellationToken ct = default);
    }
}
