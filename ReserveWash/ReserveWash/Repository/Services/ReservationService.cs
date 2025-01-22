using ReserveWash.Models;

namespace ReserveWash.Repository.Services
{
    public class ReservationService : GenericRepository<Reservation>
    {
        public ReservationService(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
