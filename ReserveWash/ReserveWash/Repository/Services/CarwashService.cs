using ReserveWash.Models;
namespace ReserveWash.Repository.Services
{
    public class CarwashService : GenericRepository<Carwash>
    {
        public CarwashService(AppDbContext dbContext) : base(dbContext)
        {
        }

    }
}
