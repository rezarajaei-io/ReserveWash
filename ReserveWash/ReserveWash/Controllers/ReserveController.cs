using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReserveWash.Models;
using ReserveWash.Repository.Services;
using ReserveWash.ViewModels.Product;
using ReserveWash.Utilities;

namespace ReserveWash
{
    public class ReserveController : Controller
    {
        private readonly ReservationService _reserveService;
        private readonly CarwashService _carwashService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ReserveController(ReservationService reserveService, CarwashService carwashService, UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager)
        {
            _reserveService = reserveService;
            _signInManager = signInManager;
            _userManager = userManager;
            _carwashService = carwashService;
        }

        // Helper method to check if the user is an admin
        private async Task<bool> IsAdminAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        // GET: car
        public async Task<IActionResult> Index(string? ReserveType)
        {
            var reserveQuery = await _reserveService.GetAllAsync();
            reserveQuery = reserveQuery
                .Include(i => i.ReserveTime.Service)
                .Include(i => i.Car)
                .Include(i => i.ReserveTime.Carwash);

            TypeAdapterConfig<Reservation, ReservationViewModel>
                .NewConfig()
                .Map(dest => dest.CarName, src => src.Car.Brand)
                .Map(dest => dest.CarPelak, src => src.Car.Pelak)
                .Map(dest => dest.CarwashId, src => src.ReserveTime.CarwashId)
                .Map(dest => dest.CarwashName, src => src.ReserveTime.Carwash.Name)
                .Map(dest => dest.ServiceName, src => src.ReserveTime.Service.Name)
                .Map(dest => dest.ReserveDateFa, src => DateConverter.GregorianToJalaliStringWithTime(src.ReserveTime.ReservationDate));


            // If user is admin, show all reservations
            if (await IsAdminAsync())
            {
                var allReservations = reserveQuery.ToList().Adapt<List<ReservationViewModel>>();
                return View(allReservations);
            }

            // For regular users, filter by ownership
            var thisUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var filtredReserves = new List<ReservationViewModel>();
            if (!string.IsNullOrEmpty(ReserveType))
            {
                filtredReserves = reserveQuery
                .Where(w => w.ReserveTime.Carwash.UserId == thisUser)
                .ToList()
                .Adapt<List<ReservationViewModel>>();
            }

            else
            {
                filtredReserves = reserveQuery
               .Where(w => w.ReserveTime.Carwash.UserId != thisUser)
               .ToList()
               .Adapt<List<ReservationViewModel>>();
            }
            return View(filtredReserves);
        }

        // GET: car/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TypeAdapterConfig<Reservation, ReservationViewModel>
                .NewConfig()
                .Map(dest => dest.CarName, src => src.Car.Brand)
                .Map(dest => dest.CarPelak, src => src.Car.Pelak)
                .Map(dest => dest.CarwashName, src => src.ReserveTime.Carwash.Name)
                .Map(dest => dest.ServiceName, src => src.ReserveTime.Service.Name)
                .Map(dest => dest.ReserveDateFa, src => DateConverter.GregorianToJalaliStringWithTime(src.ReserveTime.ReservationDate));

            var content = await _reserveService.GetByIdAsyncAsQuery((int)id, r => r.ReserveTime.Service, r => r.ReserveTime.Service, r => r.Car);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If user is admin, show details
            if (await IsAdminAsync())
            {
                var reservationDto = content.Adapt<ReservationViewModel>();
                return View(reservationDto);
            }

            // For regular users, check ownership
            var carwash = await _carwashService.GetByIdAsync(content.ReserveTime.CarwashId);
            if (carwash.UserId != currentUserId)
            {
                return Forbid();
            }

            var userReservationDto = content.Adapt<ReservationViewModel>();
            return View(userReservationDto);
        }

        // GET: car/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _reserveService.GetByIdAsyncAsQuery((int)id, r => r.ReserveTime.Service, r => r.ReserveTime.Service, r => r.Car);
            TypeAdapterConfig<Reservation, ReservationViewModel>
               .NewConfig()
               .Map(dest => dest.CarName, src => src.Car.Brand)
               .Map(dest => dest.CarPelak, src => src.Car.Pelak)
               .Map(dest => dest.CarwashName, src => src.ReserveTime.Carwash.Name)
               .Map(dest => dest.ServiceName, src => src.ReserveTime.Service.Name)
               .Map(dest => dest.ReserveDateFa, src => DateConverter.GregorianToJalaliStringWithTime(src.ReserveTime.ReservationDate));

            // If user is admin, allow deletion
            if (await IsAdminAsync())
            {
                var reservationDto = content.Adapt<ReservationViewModel>();
                return View(reservationDto);
            }

            // For regular users, check ownership
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var carwash = await _carwashService.GetByIdAsync(content.ReserveTime.CarwashId);
            if (carwash.UserId != currentUserId)
            {
                return Forbid();
            }

            var userReservationDto = content.Adapt<ReservationViewModel>();
            return View(userReservationDto);
        }

        // POST: car/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _reserveService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
