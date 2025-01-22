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

        // GET: car
        public async Task<IActionResult> Index()
        {
            var reserveQuery = await _reserveService.GetAllAsync();
            var carwashId = reserveQuery.FirstOrDefault()?.CarwashId;
            if (carwashId != null)
            {
                var carwash = await _carwashService.GetByIdAsync((int)carwashId);
                var carwashUserId = carwash.UserId;
                var thisUser = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                //var caresDto1 = reserveQuery.Where(w => carwashUserId == thisUser)
                //    .ToList()
                //    .Adapt<List<ReservationViewModel>>();

                // تعریف تنظیمات مپینگ
                TypeAdapterConfig<Reservation, ReservationViewModel>
                    .NewConfig()
                    .Map(dest => dest.CarwashName, src => src.Carwash.Name) // مپ کردن یک فیلد خاص
                    .Map(dest => dest.ServiceName, src => src.Service.Name)
                    .Map(dest => dest.ReserveDateFa, src => DateConverter.GregorianToJalaliStringWithTime(src.ReservationDate));

                var caresDto = reserveQuery
                    .Include(r => r.Carwash)
                    .Include(r => r.Service)
                    .Where(w => carwashUserId == thisUser)
                                .ToList()
                                .Adapt<List<ReservationViewModel>>();

                return View(caresDto);
            }
            List<ReservationViewModel> emptyReserveList = new List<ReservationViewModel>();
            return View(emptyReserveList);
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
                .Map(dest => dest.CarwashName, src => src.Carwash.Name) // مپ کردن یک فیلد خاص
                .Map(dest => dest.ServiceName, src => src.Service.Name)
                .Map(dest => dest.ReserveDateFa, src => DateConverter.GregorianToJalaliStringWithTime(src.ReservationDate));

            var content = await _reserveService.GetByIdAsyncAsQuery((int)id, r => r.Carwash, r => r.Service);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var carwash = await _carwashService.GetByIdAsync(content.CarwashId);
            var carwashUserId = carwash.UserId;
            var caresDto = carwashUserId == currentUserId ? content.Adapt<ReservationViewModel>() : null;

            if (caresDto == null)
            {
                return NotFound();
            }

            return View(caresDto);
        }


        // GET: car/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _reserveService.GetByIdAsync((int)id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var carwash = await _carwashService.GetByIdAsync(content.CarwashId);
            var carwashUserId = carwash.UserId;
            content = carwashUserId == currentUserId ? content : null;
            if (content == null)
            {
                return NotFound();
            }

            var ReservationViewModel = content.Adapt<ReservationViewModel>();
            return View(ReservationViewModel);
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
