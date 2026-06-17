using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.PlanViewModels;
using GymManagementSystem.Context;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Controllers
{
    public class PlanController : Controller
    {
        private readonly IPlanService _planService;

        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }
        public async Task<IActionResult> Index(CancellationToken token)
        {
            var plans = await _planService.GetAllPlansAsync();

            return View(plans);
        }
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanDetailsAsync(id,ct);
            if (plan == null)
            {
                TempData["Error"] = "Plan is not Found";
                RedirectToAction(nameof(Index));  
            }
            
           return View(plan);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanToUpdateAsync(id,ct);
            if (plan == null)
            {
                TempData["Error"] = "Plan is not Found";
                RedirectToAction(nameof(Index));
            }

            return View(plan);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdatePlanViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Failed to Edit Plan";
                return RedirectToAction(nameof(Edit), model);
            }

            var success = await _planService.UpdatePlanDetailsAsync(id, model, ct);
            if (! success)
            {
                TempData["Error"] = "Failed to Edit Plan";
                return RedirectToAction(nameof(Edit), model);
            }

            TempData["Success"] = "Plan Updated Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanDetailsAsync(id,ct);
            if (plan == null)
            {
                TempData["Error"] = "Plan is not Found";
                RedirectToAction(nameof(Index));
            }

            bool success = false;
            success = await _planService.ActivationPlanAsync(id, ct);
            if (success)
            {
                if (plan.IsActive)
                    TempData["Success"] = "Plan Activated Successfully";
                else
                    TempData["Success"] = "Plan Deactivated Successfully";
            }
            else
                TempData["Error"] = "Failed to Edit Plan";

            return RedirectToAction(nameof(Index));
        }
    }
}
