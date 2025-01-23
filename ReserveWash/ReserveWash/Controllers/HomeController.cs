using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReserveWash.BLL;
using ReserveWash.Models;
using ReserveWash.Repository.Contracts;
using ReserveWash.Repository.Services;
using ReserveWash.ViewModels.Product;
using System.Diagnostics;
using System.Security.Claims;

namespace ReserveWash.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CarService _carservice;
        private readonly ReservationService _reserveService;
        private readonly CarwashService _carwashService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, CarwashService carwashservice, CarService carservice, UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager, ReservationService reserve)
        {
            _logger = logger;
            _carservice = carservice;
            _userManager = userManager;
            _carwashService = carwashservice;
            _signInManager = signInManager;
            _reserveService = reserve;
        }

        // Carwash
        public async Task<IActionResult> Index()
        {
            var carwashes = await _carwashService.GetAllAsync();
            var carwashesDto = carwashes.ToList().Adapt<List<CarWashViewModel>>();
            var cars = await _carservice.GetAllAsync();
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var carList = cars.Where(w => w.User.Id == userId).Select
                (x => new SelectListItem { Value = x.Id.ToString(), Text = x.Brand }).ToList();
            ViewBag.Cars = carList;

            return View(carwashesDto); // ارسال محصولات به ویو
        }

    }
}