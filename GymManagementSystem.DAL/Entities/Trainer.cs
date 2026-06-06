using GymManagementSystem.DAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Entities
{
    public class Trainer : GymUser
    {
        public Specialties Specialize { get; set; }

        #region Relationships
        public ICollection<Session> Sessions { get; set; } = default!;
        #endregion
    }
}
