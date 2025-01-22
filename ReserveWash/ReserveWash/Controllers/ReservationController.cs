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
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ReservationController(ILogger<HomeController> logger, CarwashService carwashservice, CarService carservice, UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager, ServiceRepository serviceRepository, ReservationService reserve, ReserveTimeService reservetimeService)
        {
            _logger = logger;
            _carwashService = carwashservice;
            _carservice = carservice;
            _userManager = userManager;
            _signInManager = signInManager;
            _reserveService = reserve;
            _reserveTimeService = reservetimeService;
            _serviceRepository = serviceRepository;
        }

    
        public async Task<IActionResult> Index(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var reserveQuery = await _reserveTimeService.GetAllAsync();
            var thisUser = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservationTimeDto = reserveQuery.Include(i => i.Carwash)
                .Include(i => i.Service)
                .Where(w => w.Carwash.UserId == thisUser && w.CarwashId == Id && !w.IsReserved)
                .ToList();

            TypeAdapterConfig<ReserveTime, ReserveTimeViewModel>
              .NewConfig()
              .Map(dest => dest.CarwashName, src => src.Carwash.Name) // مپ کردن یک فیلد خاص
              .Map(dest => dest.ServiceName, src => src.Service.Name)
              .Map(dest => dest.ReservationDateFa, src => DateConverter.GregorianToJalaliStringWithTime(src.ReservationDate));

            return View(reservationTimeDto.Adapt<List<ReserveTimeViewModel>>());

        }


        // GET: CarWash/Create
        public async Task<IActionResult> Create(int? id)
        {
            var services = await _serviceRepository.GetAllAsync();
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            services =   services.Include(i => i.Carwash)
                .ThenInclude(t => t.User);

            var servicesList = services
                .Where(w => w.Carwash.User.Id == userId)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                .ToList();

            ViewBag.Id = id;
            ViewBag.Services = servicesList;
            ViewBag.CarwashName = services.Where(w => w.Carwash.Id == id)
                .Select(s => s.Carwash)
                .FirstOrDefault().Name;
            return View();
        }

        // POST: Reserve/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    DateTime dateOnly = DateTime.ParseExact(
                       convertedDate.ToString().Split(' ')[0], // فقط تاریخ را جدا می‌کنیم
                       "M/d/yyyy", // فرمت تاریخ با توجه به ورودی
                       System.Globalization.CultureInfo.InvariantCulture
                   );

                    // ترکیب تاریخ و زمان
                    DateTime finalDateTime = DateTime.ParseExact(
                        $"{dateOnly.ToString("yyyy-MM-dd")} {reserveTimeViewModel.ReservationTime}",
                        "yyyy-MM-dd HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture
                    );
                    reserveTime.ReservationDate = finalDateTime;
                    reserveTime.ServiceId = (int)reserveTimeViewModel.ServiceId;
                    reserveTime.CarwashId = (int)reserveTimeViewModel.CarwashId;
                    await _reserveTimeService.AddAsync(reserveTime);
                    return RedirectToAction("Index", new {Id = reserveTime.CarwashId});
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("فرمت ورودی اشتباه است: " + ex.Message);
                }
            }

            return View(reserveTimeViewModel);
        }

        // GET: car/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var content = await _reserveTimeService.GetByIdAsyncAsQuery((int)id, r => r.Carwash, r => r.Service);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservation = content.Carwash.UserId == currentUserId ? content : null;

            if (reservation == null)
            {
                return NotFound();
            }

            TypeAdapterConfig<ReserveTime, ReserveTimeViewModel>
                .NewConfig()
                .Map(dest => dest.CarwashName, src => src.Carwash.Name) // مپ کردن یک فیلد خاص
                .Map(dest => dest.ServiceName, src => src.Service.Name)
                .Map(dest => dest.ReservationDateFa, src => DateConverter.GregorianToJalaliStringWithTime(src.ReservationDate));

            return View(reservation.Adapt<ReservationViewModel>());
        }


        // GET: car/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _reserveTimeService.GetByIdAsyncAsQuery((int)id, r => r.Carwash, r => r.Service);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var carwash = await _reserveTimeService.GetByIdAsync(content.CarwashId);
            content = content.Carwash.UserId == currentUserId ? content : null;
            if (content == null)
            {
                return NotFound();
            }

            TypeAdapterConfig<ReserveTime, ReserveTimeViewModel>
               .NewConfig()
               .Map(dest => dest.CarwashName, src => src.Carwash.Name) // مپ کردن یک فیلد خاص
               .Map(dest => dest.ServiceName, src => src.Service.Name)
               .Map(dest => dest.ReservationDateFa, src => DateConverter.GregorianToJalaliStringWithTime(src.ReservationDate));
            
            var ReservationViewModel = content.Adapt<ReserveTimeViewModel>();
            ViewBag.CarwashId = content.CarwashId;
            return View(ReservationViewModel);
        }

        // POST: car/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int CarwashId)
        {
            await _reserveTimeService.DeleteAsync(id);
            return RedirectToAction(nameof(Index), new { Id = CarwashId });
        }
    }
}