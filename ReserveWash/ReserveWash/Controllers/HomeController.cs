using Mapster;
using Microsoft.AspNetCore.Mvc;
using ReserveWash.Models;
using ReserveWash.Repository.Contracts;
using ReserveWash.Repository.Services;
using ReserveWash.ViewModels.Product;
using System.Diagnostics;

namespace ReserveWash.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CarwashService _carwashService;

        public HomeController(ILogger<HomeController> logger, CarwashService carwashservice)
        {
            _logger = logger;
            _carwashService = carwashservice;
        }

        // Carwash
        public async Task<IActionResult> Index()
        {
            var carwashes = await _carwashService.GetAllAsync();
            var carwashesDto = carwashes.ToList().Adapt<List<CarWashViewModel>>();
            return View(carwashesDto); // ارسال محصولات به ویو
        }


        public IActionResult Privacy()
        {
            return View();
        }
    }
}