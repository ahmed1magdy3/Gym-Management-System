using GymManagementSystem.Context;
using GymManagementSystem.DAL.Entities;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class PlanRepository : IPlanRepository
    {
        private GymDbContext dbContext;
        public PlanRepository(GymDbContext _dbContext)
        {
            dbContext = _dbContext;
        }
        
        public async Task<IEnumerable<Plan>> GetAll(bool isTracked, CancellationToken ct = default)
        {
            var plans = isTracked? dbContext.Plans : dbContext.Plans.AsNoTracking();
            return await plans.ToListAsync();
        }

        public async Task<Plan?> GetPlanById(int id, CancellationToken ct = default)
        {
            var plan = await dbContext.Plans.FirstOrDefaultAsync(p => p.Id == id);
            return plan;
        }

        public void AddPlan(Plan plan)
        {
             dbContext.Plans.Add(plan);
        }

        public async void DeletePlan(int id)
        {
            var plan = await dbContext.Plans.FirstOrDefaultAsync(p => p.Id == id);
            if (plan != null) 
                dbContext.Plans.Remove(plan);
        }

        public void UpdatePlan(Plan plan)
        {
            dbContext.Plans.Update(plan);
        }

        public async Task<int> CompleteAsync()
        {
            return await dbContext.SaveChangesAsync();
        }

    }
}
