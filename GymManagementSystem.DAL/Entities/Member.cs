namespace GymManagementSystem.DAL.Entities
{
    public class Member : GymUser
    {
        public string? Photo { get; set; } = default!;


        #region Relationships
        public HealthRecord HealthRecord { get; set; } = default!;
        public ICollection<MemberShip> memberShips { get; set; } = default!;
        public ICollection<Booking> Bookings { get; set; } = default!;
        #endregion

    }
}