using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymManagementSystem.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        public async Task<IActionResult> Index( CancellationToken ct = default)
        {
            var sessions = await _sessionService.GetAllSessionAsync(false, ct);
            
            return View(sessions);
        }

        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropDownAsync(ct);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model ,CancellationToken ct)
        {
            if (! ModelState.IsValid)
            {
                await PopulateDropDownAsync(ct);
                return View(model);
            }

            var result = await _sessionService.CreateSessionAsync(model, ct);

            if (result.success)
            {
                TempData["Success"] = "Session Created Successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = result.ErrorMessage;
            await PopulateDropDownAsync(ct);

            return View(model);
        }

        private async Task PopulateDropDownAsync(CancellationToken ct)
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(ct),"Id","Name");
            ViewBag.Categories = new SelectList(await _sessionService.GetCategoriesForDropDownAsync(ct),"Id","CategoryName");
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var session = await _sessionService.GetSessionById(id, ct);

            if (session == null)
            {
                TempData["Error"] = "Session not Found";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropDownAsync(ct);
            return View(session);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var session = await _sessionService.GetSessionToUpdateAsync(id, ct);
            await PopulateDropDownAsync(ct);

            if (session == null)
            {
                TempData["Error"] = "Session not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(session);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Failed to Update Session";
                await PopulateDropDownAsync(ct);
                return View(model);
            }

            var result = await _sessionService.UpdateSessionAsync(id, model, ct);

            if (! result.success)
            {
                TempData["Error"] = result.ErrorMessage;
                await PopulateDropDownAsync(ct);
                return View(model);
                
            }

            TempData["Success"] = "Session Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete (int id, CancellationToken ct) 
        {
            var session = await _sessionService.GetSessionById(id, ct);
            if (session == null)
            {
                TempData["Error"] = "Session not Found";
                RedirectToAction(nameof(Index));
            }

            return View(session);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _sessionService.RemoveSessionAsync(id, ct);

            if (result.success)
                TempData["Success"] = "Session Deleted Successfully";

            else
                TempData["Error"] = result.ErrorMessage;


            return RedirectToAction(nameof(Index));
        }

    }
}

