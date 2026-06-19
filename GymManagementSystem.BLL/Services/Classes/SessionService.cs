using AutoMapper;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.SessionViewModels;
using GymManagementSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<IEnumerable<SessionViewModel>> GetAllSessionAsync(bool isTracked = false, CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.sessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(ct);

            if (!sessions.Any()) return [];

            sessions = sessions.OrderByDescending(s => s.StartDate);

            //var sessionViewModel = sessions.Select(s => new SessionViewModel()
            //{
            //    Id = s.Id,
            //    Description = s.Description,
            //    Capacity = s.Capacity,
            //    StartDate = s.StartDate,
            //    EndDate = s.EndDate,
            //    TrainerName = s.Trainer.Name,
            //    CategoryName = s.Category.CategoryName,
            //});

            var sessionViewModel = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);

            foreach (var session in sessionViewModel)
            {
                session.AvailableSlots = session.Capacity - await _unitOfWork.sessionRepository.GetCountOfBookedSlotAsync(session.Id, ct);
            }

            return sessionViewModel;
        }

        public async Task<bool> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct)
        {
            if (model.EndDate <= model.StartDate) return false;

            if (model.StartDate <= DateTime.Now) return false;

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetById(model.TrainerId,ct);
            if(trainer == null) return false;

            var category = await _unitOfWork.GetRepository<Category>().GetById(model.CategoryId,ct);
            if (category == null) return false;

            var session = _mapper.Map<Session>(model);

            
            _unitOfWork.GetRepository<Session>().Add(session);
            var result = await _unitOfWork.CompleteAsync();

            return result > 0;
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAll(false,ct);

            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(trainers);
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct)
        {
            var categries = await _unitOfWork.GetRepository<Category>().GetAll(false,ct);

            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(categries);
        }
    }
}
