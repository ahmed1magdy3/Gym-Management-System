using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.MemberViewModels;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class MemberServices : IMemberServices
    {
        private readonly IGenericRepository<Member> _memberRepository;
        private readonly IGenericRepository<MemberShip> _membershipRepository;
        private readonly IGenericRepository<Plan> _planRepository;
        private readonly IGenericRepository<HealthRecord> _healthRecordRepository;
        private readonly IGenericRepository<Booking> _bookingRepository;


        // Get
        public MemberServices(IGenericRepository<Member> memberRepository, 
                              IGenericRepository<MemberShip> membershipRepository,
                              IGenericRepository<Plan>planRepository,
                              IGenericRepository<HealthRecord>healthRecordRepository,
                              IGenericRepository<Booking>bookingRepository)
        {
            _memberRepository = memberRepository;
            _membershipRepository = membershipRepository;
            _planRepository = planRepository;
            _healthRecordRepository = healthRecordRepository;
            _bookingRepository = bookingRepository;
        }
        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _memberRepository.GetAll(false, ct);

            if (!members.Any()) return [];

            var MembersViewModel = members.Select(m => new MemberViewModel()
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                Photo = m.Photo,
                Gender = m.Gender.ToString()
            });
            return MembersViewModel;
        }

        public async Task<MemberViewModel?> GetDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetById(memberId);
            if (member == null) return null;

            var MemberViewModel = new MemberViewModel()
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                DateOfBirth = member.DateOfBirth.ToString(),
                Gender = member.Gender.ToString(),
                Address = $"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City}"

            };
            // MemberShip
             var ActiveMemberShip = await _membershipRepository.FirstOrDefaultAsync(mb => mb.MemberId== memberId && mb.EndDate > DateTime.Now, false, ct);
            
            if (ActiveMemberShip is not null)
            {
                var ActivePlan = await _planRepository.GetById(ActiveMemberShip.PlanId, ct);
                MemberViewModel.PlanName = ActivePlan?.Name;
                MemberViewModel.MembershipStartDate = ActiveMemberShip.CreatedAt.ToShortDateString();
                MemberViewModel.MembershipEndDate = ActiveMemberShip.EndDate.ToShortDateString(); 
            }

            return MemberViewModel;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var record = await _healthRecordRepository.FirstOrDefaultAsync(r => r.MemberId == memberId, false, ct);
            if (record is null) return null;

            return new HealthRecordViewModel()
            {
                Weight = record.Weight,
                Height = record.Height,
                BloodType = record.BloodType,
                Note = record.Note
            };}

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetById(memberId,ct);
            if (member is null) return null;

            return new MemberToUpdateViewModel()
            {
                Name = member.Name,
                Email = member.Email,
                Phone = member.Phone,
                Street = member.Address.Street,
                City = member.Address.City,
                BuildingNumber = member.Address.BuildingNumber,
                Photo = member.Photo
            };
        }

        // Post
        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct)
        {
            var emailExits = await _memberRepository.AnyAsync(m => m.Email == model.Email, ct);
            var phoneExits = await _memberRepository.AnyAsync(m => m.Phone == model.Phone, ct);

            if (emailExits || phoneExits) return false;

            var member = new Member()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = new Address
                {
                    BuildingNumber = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street,
                },
                HealthRecord = new HealthRecord()
                {
                    Weight = model.HealthRecordViewModel.Weight,
                    Height = model.HealthRecordViewModel.Height,
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Note = model.HealthRecordViewModel.Note
                }
            };

            _memberRepository.Add(member);
            var result = await _memberRepository.CompleteAsync();

            return result > 0;
        }

        public async Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetById(id, ct);
            if (member == null) return false;


            var emailExits = await _memberRepository.AnyAsync(m => m.Email == model.Email && m.Id != id, ct);
            var phoneExits = await _memberRepository.AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct);
            if (emailExits || phoneExits) return false;

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.UpdatedAt = DateTime.Now;

            _memberRepository.Update(member);

            var result = await _memberRepository.CompleteAsync();

            return result > 0;

        }
        public async Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct = default)
        {
            var member = _memberRepository.GetById(memberId, ct);
            if (member == null) return false;

            var futureSession = await _bookingRepository.AnyAsync(b => b.MemberId == memberId && b.Session.StartDate > DateTime.Now, ct);
            if (futureSession) return false;

            _memberRepository.Delete(memberId);

            var result = await _memberRepository.CompleteAsync();
            return result > 0;

        }
        

        
    }
}
