using GymManagementSystem.Context;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Controllers
{
    public class PlanController : Controller
    {
        private readonly IGenericRepository<Plan> planRepository;
        public PlanController(IGenericRepository<Plan> _planRepository)
        {
            planRepository = _planRepository;
        }
        public async Task<IActionResult> Index(CancellationToken token)
        {
            var plans = await planRepository.GetAll(isTracked:false, ct:token);

            return View(plans);
        }
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await planRepository.GetById(id, ct);
            if (plan == null)
            { 
                RedirectToAction(nameof(Index));  
            }
            
           return View(plan);
        }
    }
}
