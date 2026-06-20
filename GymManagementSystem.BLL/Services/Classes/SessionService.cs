using AutoMapper;
using GymManagementSystem.BLL.Services.Common;
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

        // Get
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

        public async Task<SessionViewModel?> GetSessionById(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.sessionRepository.GetSessionWithTrainerAndCategoryAsync(sessionId, ct);

            if (session == null) return null;

            var sessionViewModel = _mapper.Map<SessionViewModel>(session);
            sessionViewModel.AvailableSlots = sessionViewModel.Capacity - await _unitOfWork.sessionRepository.GetCountOfBookedSlotAsync(sessionId,ct);

            return sessionViewModel;
        }

        public async Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.sessionRepository.GetById(sessionId, ct);

            if (session == null) return null;

            if (!await IsSessionValidToUpdate(session, ct)) return null;

            var updateSession = _mapper.Map<UpdateSessionViewModel>(session);
            return updateSession;
        }


        // Post
        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct)
        {
            if (model.EndDate <= model.StartDate) return Result.Validation("End Date must be after Start Date");

            if (model.StartDate <= DateTime.Now) return Result.Validation("Start Date must be in the Future");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetById(model.TrainerId, ct);
            if (trainer == null) return Result.NotFound("Trainer not found");

            var category = await _unitOfWork.GetRepository<Category>().GetById(model.CategoryId, ct);
            if (category == null) return Result.NotFound("Category not found");

            var session = _mapper.Map<Session>(model);


            _unitOfWork.GetRepository<Session>().Add(session);
            var rowEffected = await _unitOfWork.CompleteAsync();

            return rowEffected > 0 ? Result.Ok() : Result.Fail("Failed to create session");
        }

        public async Task<Result> UpdateSessionAsync(int sessionId, UpdateSessionViewModel model ,CancellationToken ct = default)
        {
            var session = await _unitOfWork.sessionRepository.GetById(sessionId, ct);
            if (session == null) return Result.NotFound("session not found");

            if (session.StartDate <= DateTime.Now) return Result.Validation("Start Date must be in the Future");

            var booked = await _unitOfWork.sessionRepository.GetCountOfBookedSlotAsync(sessionId, ct);
            if (booked > 0) return Result.Validation("can not update a session already booked by members");

            if (model.EndDate <= model.StartDate) return Result.Validation("End Date must be after Start Date");
            if (model.StartDate <= DateTime.Now) return Result.Validation("Start Date must be in the Future");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetById(model.TrainerId, ct);
            if (trainer == null) return Result.NotFound("Trainer not found");

            session.UpdatedAt = DateTime.Now;

            _mapper.Map(model,session);

            _unitOfWork.sessionRepository.Update(session);

            var rowEffected =await _unitOfWork.CompleteAsync();

            return rowEffected > 0 ? Result.Ok() : Result.Fail("Failed to update session");
        }


        private async Task<bool> IsSessionValidToUpdate(Session session, CancellationToken ct)
        {
            if (session.StartDate <= DateTime.Now) return false;

            var booked = await _unitOfWork.sessionRepository.GetCountOfBookedSlotAsync(session.Id, ct);

            return booked == 0;
        }

        public async Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.sessionRepository.GetById(sessionId, ct);
            if (session == null) return Result.NotFound("session not found");

            if ( session.StartDate <= DateTime.Now && session.EndDate >= DateTime.Now) return Result.Validation("can not delete sesssion that started and not yet ended");

            var booked = await _unitOfWork.sessionRepository.GetCountOfBookedSlotAsync(sessionId,ct);
            if (booked > 0) return Result.Validation("can not delete a session already booked by members");

            _unitOfWork.sessionRepository.Delete(sessionId);

            var rowEffected = await _unitOfWork.CompleteAsync();
            return rowEffected > 0 ? Result.Ok() : Result.Fail("Failed to delete session");
        }


    }
}
