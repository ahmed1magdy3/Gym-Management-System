using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Entities
{
    public class Booking : BaseEntity
    {
        #region Relationship
        public Member Member { get; set; } = default!;
        public int MemberId { get; set; }

        public Session Session { get; set; } = default!;
        public int SessionId { get; set; }
        #endregion

        public bool IsAttended {  get; set; }
    }
}
