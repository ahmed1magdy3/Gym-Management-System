using GymManagementSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface IPlanRepository
    {
        Task<IEnumerable<Plan>> GetAll(bool isTracked, CancellationToken ct=default);
        Task<Plan?> GetPlanById(int id, CancellationToken ct = default);
        void AddPlan(Plan plan);
        void UpdatePlan(Plan plan);
        void DeletePlan(int id);
        Task<int> CompleteAsync();
    }
}
