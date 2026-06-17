using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Controllers
{
    public class MemberController : Controller
    {
        private readonly IMemberServices _memberServices;

        public MemberController(IMemberServices memberServices)
        {
            _memberServices = memberServices;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberServices.GetAllMembersAsync(ct);

            return View(members);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create) ,model);

            var result = await _memberServices.CreateMemberAsync(model,ct);

            if (result)
                TempData["Success"] = "Member Created Successfully";
            else
                TempData["Error"] = "Failed to Create Member";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await _memberServices.GetDetailsAsync(id,ct);
            if (member is null)
            {
                TempData["Error"] = "Member is not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        [HttpGet]
        public async Task<ActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var healthRecord = await _memberServices.GetMemberHealthRecordAsync(id,ct);
            if(healthRecord is null)
            {
                TempData["Error"] = "Health Record is not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(healthRecord);
        }

        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var member = await _memberServices.GetMemberToUpdateAsync(id,ct);
            if (member is null)
            {
                TempData["Error"] = "Member is not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> EditMember(int id ,MemberToUpdateViewModel member,CancellationToken ct)
        {
            if (! ModelState.IsValid)
            {
                TempData["Error"] = "Failed to Edit Member";
                return RedirectToAction(nameof(EditMember), member);
            }
            var success =  await _memberServices.UpdateMemberDetailsAsync(id,member, ct);
            if (!success)
            {
                TempData["Error"] = "Failed to Edit Member";
                return View(member);
            }
            
            TempData["Success"] = "Member Updated Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var member = await _memberServices.GetDetailsAsync(id,ct);
            if (member is null)
            {
                TempData["Error"] = "Member is not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var success = await _memberServices.DeleteMemberAsync(id,ct);
            if (!success)
                TempData["Error"] = "Failed to Delete Member";
            else
                TempData["Success"] = "Member Deleted Successfully";

            return RedirectToAction(nameof(Index));
        }

    }
}
