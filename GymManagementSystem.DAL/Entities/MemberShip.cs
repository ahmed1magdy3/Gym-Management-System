using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Entities
{
    public class MemberShip : BaseEntity
    {

        #region Relationship
        public Member Member { get; set; } = default!;
        public int MemberId { get; set; } // FK

        public Plan Plan { get; set; } = default!;
        public int PlanId { get; set; } // FK
        #endregion

        public DateTime EndDate { get; set; }

        public string Status => EndDate > DateTime.Now ? "Active" : "Expired";

        public bool IsActive => EndDate > DateTime.Now;
    }
}
