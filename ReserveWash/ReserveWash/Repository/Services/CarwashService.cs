using ReserveWash.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace ReserveWash.Repository.Services
{
    public class CarwashService : GenericRepository<Carwash>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public CarwashService(UserManager<ApplicationUser> userManager, AppDbContext dbContext) : base(dbContext)
        {
            _userManager = userManager;
        }

        public void ReserverCarwash()
        {
            var userId = _userManager.FindByIdAsync(ClaimTypes.NameIdentifier);
        }
    }
}
