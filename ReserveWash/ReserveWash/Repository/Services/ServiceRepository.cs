using ReserveWash.Models;

namespace ReserveWash.Repository.Services
{
    public class ServiceRepository : GenericRepository<Service>
    {
        public ServiceRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
