using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReserveWash.BLL;
using ReserveWash.Models;
using ReserveWash.Repository.Services;
using ReserveWash.Utilities;
using ReserveWash.ViewModels.Product;
using System.Security.Claims;

namespace ReserveWash.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CarwashService _carwashService;
        private readonly CarService _carservice;
        private readonly ReservationService _reserveService;
        private readonly ServiceRepository _serviceRepository;
        private readonly ReserveTimeService _reserveTimeService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationController(ILogger<HomeController> logger, CarwashService carwashservice, CarService carservice, UserManager<ApplicationUser> userManager,
            ServiceRepository serviceRepository, ReservationService reserve, ReserveTimeService reservetimeService)
        {
            _logger = logger;
            _carwashService = carwashservice;
            _carservice = carservice;
            _userManager = userManager;
            _reserveService = reserve;
            _reserveTimeService = reservetimeService;
            _serviceRepository = serviceRepository;
        }

        private async Task<bool> IsAdminAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<IActionResult> Index(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var reserveQuery = await _reserveTimeService.GetAllAsync();
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = await IsAdminAsync();
            var reservationTimeDto = await reserveQuery.Include(i => i.Carwash)
                .Include(i => i.Service)
                .Where(w => (isAdmin || w.Carwash.UserId == currentUserId) && w.CarwashId == Id && !w.IsReserved)
                .ToListAsync();

            TypeAdapterConfig<ReserveTime, ReserveTimeViewModel>
              .NewConfig()
              .Map(dest => dest.CarwashName, src => src.Carwash.Name)
              .Map(dest => dest.ServiceName, src => src.Service.Name)
              .Map(dest => dest.ReservationDateFa, src => DateConverter.GregorianToJalaliStringWithTime(src.ReservationDate));

            return View(reservationTimeDto.Adapt<List<ReserveTimeViewModel>>());
        }

        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var services = await _serviceRepository.GetAllAsync();
            services = services.Include(i => i.Carwash);

            var servicesList = services
                .Where(w => w.Carwash.Id == id)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToList();

            ViewBag.Id = id;
            ViewBag.Services = servicesList;

            var carwash = await _carwashService.GetByIdAsync((int)id);
            ViewBag.CarwashName = carwash?.Name;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReserveTimeViewModel reserveTimeViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var reserveTime = reserveTimeViewModel.Adapt<ReserveTime>();
                    var convertedDate = DateConverter.ConvertJalaliToGregorian(reserveTimeViewModel.ReservationDateFa);
                    reserveTime.ReservationDate = DateTime.ParseExact($"{convertedDate:yyyy-MM-dd} {reserveTimeViewModel.ReservationTime}", "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    reserveTime.ServiceId = reserveTimeViewModel.ServiceId ?? 0;
                    reserveTime.CarwashId = reserveTimeViewModel.CarwashId ?? 0;

                    await _reserveTimeService.AddAsync(reserveTime);
                    return RedirectToAction("Index", new { Id = reserveTime.CarwashId });
                }
                catch (FormatException ex)
                {
                    ModelState.AddModelError(string.Empty, "Invalid date format: " + ex.Message);
                }
            }

            return View(reserveTimeViewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _reserveTimeService.GetByIdAsyncAsQuery((int)id, r => r.Carwash, r => r.Service);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (content == null || (!await IsAdminAsync() && content.Carwash.UserId != currentUserId))
            {
                return Forbid();
            }

            TypeAdapterConfig<ReserveTime, ReserveTimeViewModel>
                .NewConfig()
                .Map(dest => dest.CarwashName, src => src.Carwash.Name)
                .Map(dest => dest.ServiceName, src => src.Service.Name)
                .Map(dest => dest.ReservationDateFa, src => DateConverter.GregorianToJalaliStringWithTime(src.ReservationDate));

            return View(content.Adapt<ReserveTimeViewModel>());
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _reserveTimeService.GetByIdAsyncAsQuery((int)id, r => r.Carwash, r => r.Service);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (content == null || (!await IsAdminAsync() && content.Carwash.UserId != currentUserId))
            {
                return Forbid();
            }

            TypeAdapterConfig<ReserveTime, ReserveTimeViewModel>
               .NewConfig()
               .Map(dest => dest.CarwashName, src => src.Carwash.Name)
               .Map(dest => dest.ServiceName, src => src.Service.Name)
               .Map(dest => dest.ReservationDateFa, src => DateConverter.GregorianToJalaliStringWithTime(src.ReservationDate));

            ViewBag.CarwashId = content.CarwashId;
            return View(content.Adapt<ReserveTimeViewModel>());
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int carwashId)
        {
            var content = await _reserveTimeService.GetByIdAsync(id);

            if (content == null || (!await IsAdminAsync() && content.Carwash.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                return Forbid();
            }

            await _reserveTimeService.DeleteAsync(id);
            return RedirectToAction(nameof(Index), new { Id = carwashId });
        }

        [HttpPost]
        public async Task<IActionResult> GetAllByCarWashId(int carwashId)
        {
            var reserveTimeModel = await _reserveTimeService.GetAllAsync();
            reserveTimeModel = reserveTimeModel.Include(i => i.Carwash)
                .Include(i => i.Service)
                .Where(w => w.CarwashId == carwashId && !w.IsReserved);

            TypeAdapterConfig<ReserveTime, ReserveTimeViewModel>
                 .NewConfig()
                 .Map(dest => dest.CarwashName, src => src.Carwash.Name)
                 .Map(dest => dest.ServiceName, src => src.Service.Name)
                 .Map(dest => dest.ReservationDateFa, src => DateConverter.GregorianToJalaliStringWithTime(src.ReservationDate));

            if (!reserveTimeModel.Any())
            {
                return Content("<h5>در حال حاضر زمانی برای رزرو این کارواش  ثبت نشده !</h5>");
            }

            return PartialView("ReserveTimeCarwashView", reserveTimeModel.Adapt<List<ReserveTimeViewModel>>());
        }

        [HttpPost]
        public async Task<bool> InsertReservation(int? carId, int? reserveTimeId)
        {
            if (carId == null || reserveTimeId == null)
            {
                return false;
            }

            try
            {
                var reserveItems = ReservationBLL.MakeReserveModelItems((int)carId, (int)reserveTimeId);
                var reserveTimeItem = await _reserveTimeService.GetByIdAsync((int)reserveTimeId);

                if (reserveTimeItem != null)
                {
                    reserveTimeItem.IsReserved = true;
                    await _reserveTimeService.UpdateAsync(reserveTimeItem);
                    await _reserveService.AddAsync(reserveItems);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
