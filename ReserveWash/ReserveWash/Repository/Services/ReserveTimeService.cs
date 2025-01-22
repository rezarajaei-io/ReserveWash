using ReserveWash.Models;

namespace ReserveWash.Repository.Services
{
    public class ReserveTimeService : GenericRepository<ReserveTime>
    {
        public ReserveTimeService(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
