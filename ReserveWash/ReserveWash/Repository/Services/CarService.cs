using ReserveWash.Models;

namespace ReserveWash.Repository.Services
{
    public class CarService : GenericRepository<Car>
    {
        public CarService(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
