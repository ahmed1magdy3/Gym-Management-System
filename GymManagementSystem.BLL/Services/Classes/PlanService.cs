using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.PlanViewModels;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IGenericRepository<Plan> _planRepository;
        private readonly IGenericRepository<MemberShip> _membershipRepository;

        public PlanService(IGenericRepository<Plan> planRepository,
                            IGenericRepository<MemberShip> memberShipRepository)
        {
            _planRepository = planRepository;
            _membershipRepository = memberShipRepository;
        }

        // Get
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _planRepository.GetAll(false,ct);
            if (!plans.Any()) return [];

            var planViewModel = plans.Select(p => new PlanViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                DurationDays = p.DurationDays,
                Description = p.Description,
                Price = p.Price,
                IsActive = p.IsActive
            });
            
            return planViewModel;
        }

        public async Task<PlanViewModel?> GetPlanDetailsAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetById(planId,ct);
            if (plan == null) return null;

            var planViewModel = new PlanViewModel()
            {
                Id = plan.Id,
                Name = plan.Name,
                DurationDays = plan.DurationDays,
                Description = plan.Description,
                Price = plan.Price,
                IsActive = plan.IsActive
            };

            return planViewModel;
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetById(planId,ct);
            if (plan == null) return null;

            var updatePlanViewModel = new UpdatePlanViewModel()
            {
                PlanName = plan.Name,
                DurationDays = plan.DurationDays,
                Description = plan.Description,
                Price = plan.Price
            };

            return updatePlanViewModel;
        }

        // Post

        public async Task<bool> UpdatePlanDetailsAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetById(id, ct);
            if (plan == null) return false;

            var ActiveMemberShip = await _membershipRepository.FirstOrDefaultAsync(mb => mb.PlanId == id && mb.EndDate > DateTime.Now, false, ct);
            if (plan.IsActive && ActiveMemberShip != null) return false;

            plan.Name = model.PlanName;
            plan.DurationDays = model.DurationDays;
            plan.Description = model.Description;
            plan.Price = model.Price;
            plan.UpdatedAt = DateTime.Now;

            _planRepository.Update(plan);

            var result = await _planRepository.CompleteAsync();
            return result > 0;
        }

        public async Task<bool> ActivationPlanAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetById(planId, ct);
            if (plan == null) return false;

            var ActiveMemberShip = await _membershipRepository.FirstOrDefaultAsync(mb => mb.PlanId == planId && mb.EndDate > DateTime.Now, false, ct);
            if ( plan.IsActive && ActiveMemberShip != null) return false;

            plan.IsActive = ! plan.IsActive;

            _planRepository.Update(plan);

            var result = await _planRepository.CompleteAsync();
            return result > 0;
        }


    }
}
