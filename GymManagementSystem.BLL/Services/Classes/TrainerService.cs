using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.TrainerViewModels;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IGenericRepository<Trainer> _trainerRepository;
        private readonly IGenericRepository<Session> _sessionRepository;

        public TrainerService(IGenericRepository<Trainer> trainerRepository,
                              IGenericRepository<Session> sessionRepository)
        {
            _trainerRepository = trainerRepository;
            _sessionRepository = sessionRepository;
        }

        // Get
        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _trainerRepository.GetAll(false, ct);
            if (trainers == null) return [];

            var trainerViewModel = trainers.Select(x => new TrainerViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Phone = x.Phone,
                Photo = x.Photo,
                DateOfBirth = x.DateOfBirth.ToShortDateString(),
                Gender = x.Gender.ToString(),
                Specialties = x.Specialize.ToString(),
                Address = $"{x.Address.BuildingNumber} - {x.Address.Street} - {x.Address.City}"
            });
            
            return trainerViewModel;
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetById(trainerId, ct);
            if (trainer == null) return null;

            var trainerViewModel = new TrainerViewModel()
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Photo = trainer.Photo,
                DateOfBirth = trainer.DateOfBirth.ToShortDateString(),
                Gender = trainer.Gender.ToString(),
                Specialties = trainer.Specialize.ToString(),
                Address = $"{trainer.Address.BuildingNumber} - {trainer.Address.Street} - {trainer.Address.City}"
            };

            return trainerViewModel;
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetById(trainerId, ct);
            if (trainer == null) return null;

            var trainerToUpdate = new TrainerToUpdateViewModel()
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                BuildingNumber = trainer.Address.BuildingNumber,
                Street = trainer.Address.Street,
                City = trainer.Address.City,
                Specialties = trainer.Specialize
            };

            return trainerToUpdate;
        }

        // Post
        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct)
        {
            var emailExists = await _trainerRepository.AnyAsync(t => t.Email == model.Email);
            var phoneExists = await _trainerRepository.AnyAsync(t => t.Phone == model.Phone);
            if (emailExists || phoneExists) return false;

            var trainer = new Trainer()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street,
                    City = model.City
                },
                Specialize = model.Specialties
            };

            _trainerRepository.Add(trainer);

            var result = await _trainerRepository.CompleteAsync();
            return result > 0;
        }

        public async Task<bool> UpdateTrainerDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var emailExists = await _trainerRepository.AnyAsync(t => t.Email == model.Email && t.Id != id);
            var phoneExists = await _trainerRepository.AnyAsync(t => t.Phone == model.Phone && t.Id != id);
            if (emailExists || phoneExists) return false;

            var trainer = await _trainerRepository.GetById(id, ct);
            if(trainer == null) return false;

            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.Street = model.Street;
            trainer.Address.City = model.City;
            trainer.Specialize = model.Specialties;
            trainer.UpdatedAt = DateTime.Now;

            _trainerRepository.Update(trainer);

            var result = await _trainerRepository.CompleteAsync();
            return result > 0;
        }
        public async Task<bool> DeleteTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetById(trainerId);
            if(trainer == null) return false;

            var hasFutureSession = await _sessionRepository.AnyAsync(s => s.TrainerId == trainerId && s.StartDate > DateTime.Now);
            if(hasFutureSession) return false;

            _trainerRepository.Delete(trainerId);

            var result = await _trainerRepository.CompleteAsync();
            return result > 0;
        }

        
    }
}
