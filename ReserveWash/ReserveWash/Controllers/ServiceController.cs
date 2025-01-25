using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using ReserveWash.AI;
using ReserveWash.Models;
using ReserveWash.Repository.Services;
using ReserveWash.ViewModels.Product;
using System.Security.Claims;

namespace ReserveWash.Controllers
{
    public class ServiceController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CarwashService _carwashService;
        private readonly CarService _carservice;
        private readonly ReservationService _reserveService;
        private readonly ServiceRepository _serviceRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly MLContext _mlContext;

        public ServiceController(ILogger<HomeController> logger, CarwashService carwashservice, CarService carservice, UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager, ServiceRepository serviceRepository, ReservationService reserve)
        {
            _logger = logger;
            _carwashService = carwashservice;
            _carservice = carservice;
            _userManager = userManager;
            _signInManager = signInManager;
            _reserveService = reserve;
            _serviceRepository = serviceRepository;
            _mlContext = new MLContext();
        }

        // Helper method to check if the user is an admin
        private async Task<bool> IsAdminAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        // GET: Service
        public async Task<IActionResult> Index(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var serviceQuery = await _serviceRepository.GetAllAsync();

            // If user is admin, show all services
            if (await IsAdminAsync())
            {
                var adminServices = serviceQuery.Include(i => i.Carwash)
                    .Where(w => w.CarwashId == Id)
                    .ToList();
                var carwash = await _carwashService.GetByIdAsync((int)Id);
                ViewBag.CarwashName = carwash?.Name;
                return View(adminServices.Adapt<List<CarWashServiceViewModel?>>());
            }

            // For regular users, filter by ownership
            var thisUser = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userServices = serviceQuery.Include(i => i.Carwash)
                .Where(w => w.Carwash.UserId == thisUser && w.CarwashId == Id)
                .ToList();
            var userCarwash = await _carwashService.GetByIdAsync((int)Id);
            ViewBag.CarwashName = userCarwash?.Name;
            return View(userServices.Adapt<List<CarWashServiceViewModel?>>());
        }

        // GET: Service/Create
        public async Task<IActionResult> Create(int? id)
        {
            var carwash = await _carwashService.GetByIdAsync((int)id);

            if (carwash == null)
            {
                return NotFound();
            }

            // Allow only if the user owns the carwash or is an admin
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (carwash.UserId != currentUserId && !await IsAdminAsync())
            {
                return Forbid();
            }

            ViewBag.Id = id;
            ViewBag.CarwashName = carwash?.Name;
            return View();
        }

        // POST: Service/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarWashServiceViewModel CarWashServiceViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var service = CarWashServiceViewModel.Adapt<Service>();
                    await _serviceRepository.AddAsync(service);
                    return RedirectToAction("Index", new { Id = service.CarwashId });
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("فرمت ورودی اشتباه است: " + ex.Message);
                }
            }

            return View(CarWashServiceViewModel);
        }

        // GET: Service/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _serviceRepository.GetByIdAsyncAsQuery((int)id, r => r.Carwash);

            // Allow only if the user owns the service or is an admin
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (content.Carwash.UserId != currentUserId && !await IsAdminAsync())
            {
                return Forbid();
            }

            ViewBag.CarwashId = content?.Carwash?.Id;
            var serviceViewModel = content?.Adapt<CarWashServiceViewModel>();
            return View(serviceViewModel);
        }

        // POST: Service/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CarWashServiceViewModel serviceViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var service = serviceViewModel.Adapt<Service>();
                    await _serviceRepository.UpdateAsync(service);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    throw ex;
                }
                return RedirectToAction(nameof(Index), new { Id = serviceViewModel.CarwashId });
            }
            return View(serviceViewModel);
        }

        // GET: Service/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _serviceRepository.GetByIdAsyncAsQuery((int)id, r => r.Carwash);

            // Allow only if the user owns the service or is an admin
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (content.Carwash.UserId != currentUserId && !await IsAdminAsync())
            {
                return Forbid();
            }

            var serviceViewModel = content?.Adapt<CarWashServiceViewModel>();
            ViewBag.CarwashId = content?.CarwashId;
            return View(serviceViewModel);
        }

        // POST: Service/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int CarwashId)
        {
            var content = await _serviceRepository.GetByIdAsyncAsQuery(id, r=> r.Carwash);

            // Allow only if the user owns the service or is an admin
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (content.Carwash.UserId != currentUserId && !await IsAdminAsync())
            {
                return Forbid();
            }

            await _serviceRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index), new { Id = CarwashId });
        }

        public async Task<IActionResult> TrainModel()
        {
            var services = await _reserveService.GetAllAsync();
            services = services.Include(i => i.ReserveTime)
                .ThenInclude(t => t.Service);

            var data = services.Select(s => new CustomerServiceData()
            {
                CarId = s.CarId,
                ServiceId = s.ReserveTime.ServiceId,
                ServiceDate = s.ReserveTime.ReservationDate.Ticks,
                ServiceCost = s.ReserveTime.Service.Price
            }).ToList();

            var machineLearningService = new MachineLearningService();
            machineLearningService.TrainModel(data);
            return Content("<h1>در حال آموزش مدل...</h1>");
        }
    }
}
