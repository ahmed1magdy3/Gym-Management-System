using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace GymManagementSystem.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainerController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }
        public async Task<IActionResult> Index()
        {
            var trainers = await _trainerService.GetAllTrainersAsync();

            return View(trainers);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model,CancellationToken ct)
        {
            var result = await _trainerService.CreateTrainerAsync(model,ct);
            if (! result)
            {
                TempData["Error"] = "Failed to Create Trainer";
                return View(model);
            }

            TempData["Success"] = "Trainer Created Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id,CancellationToken ct)
        {

            var trainer = await _trainerService.GetTrainerToUpdateAsync(id,ct);
            if(trainer == null)
            {
                TempData["Error"] = "Trainer not Found";
                return RedirectToAction(nameof(Index));
            }


            return View(trainer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TrainerToUpdateViewModel model ,CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var trainer = await _trainerService.GetTrainerToUpdateAsync(id, ct);
            if (trainer == null)
            {
                TempData["Error"] = "Trainer not Found";
                return RedirectToAction(nameof(Index));
            }

            var result = await _trainerService.UpdateTrainerDetailsAsync(id, model, ct);
            if (! result)
            {
                TempData["Error"] = "Failed to Update Trainer";
                return View(model);
            }

            TempData["Success"] = "Trainer Updated Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerDetailsAsync (id, ct);

            if (trainer == null)
            {
                TempData["Error"] = "Trainer not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(trainer);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerDetailsAsync(id,ct);
            if (trainer == null)
            {
                TempData["Error"] = "Trainer not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(trainer);
        }

        // 

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _trainerService.DeleteTrainerAsync(id, ct);
            if (!result)
                TempData["Error"] = "Failed to Delete Trainer";
            else
                TempData["Success"] = "Trainer Deleted Successfully";


            return RedirectToAction(nameof(Index));
        }
    }
}
