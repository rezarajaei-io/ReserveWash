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
    public class ServiceController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CarwashService _carwashService;
        private readonly CarService _carservice;
        private readonly ReservationService _reserveService;
        private readonly ServiceRepository _serviceRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

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
        }

    
        public async Task<IActionResult> Index(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var serviceQuery = await _serviceRepository.GetAllAsync();
            var thisUser = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservationTimeDto = serviceQuery.Include(i => i.Carwash)
                .Where(w => w.Carwash.UserId == thisUser && w.CarwashId == Id )
                .ToList();
            ViewBag.CarwashName = reservationTimeDto.FirstOrDefault().Carwash?.Name;
            return View(reservationTimeDto.Adapt<List<CarWashServiceViewModel>>());

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
        public async Task<IActionResult> Create(CarWashServiceViewModel CarWashServiceViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var Service = CarWashServiceViewModel.Adapt<Service>();

                    await _serviceRepository.AddAsync(Service);
                    return RedirectToAction("Index", new {Id = Service.CarwashId});
                }
                catch (FormatException ex)
                {
                    Console.WriteLine("فرمت ورودی اشتباه است: " + ex.Message);
                }
            }

            return View(CarWashServiceViewModel);
        }

        // GET: car/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _serviceRepository.GetByIdAsyncAsQuery((int)id, r => r.Carwash);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var service = content.Carwash.UserId == currentUserId ? content : null;

            if (service == null)
            {
                return NotFound();
            }

            ViewBag.CarwashId = content.Carwash.Id;
            var serviceViewModel = service.Adapt<CarWashServiceViewModel>();
            return View(serviceViewModel);
        }

        // POST: car/Edit/5
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
                return RedirectToAction(nameof(Index), new {Id = serviceViewModel.CarwashId});
            }
            return View(serviceViewModel);
        }

        // GET: car/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var content = await _serviceRepository.GetByIdAsyncAsQuery((int)id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var service = content.Carwash.UserId == currentUserId ? content : null;

            if (service == null)
            {
                return NotFound();
            }

            return View(service.Adapt<CarWashServiceViewModel>());
        }


        // GET: car/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _serviceRepository.GetByIdAsyncAsQuery((int)id, r => r.Carwash);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            content = content.Carwash.UserId == currentUserId ? content : null;
            if (content == null)
            {
                return NotFound();
            }

            var CarWashServiceViewModel = content.Adapt<CarWashServiceViewModel>();
            ViewBag.CarwashId = content.CarwashId;
            return View(CarWashServiceViewModel);
        }

        // POST: car/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int CarwashId)
        {
            await _serviceRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index), new { Id = CarwashId });
        }
      
    }
}