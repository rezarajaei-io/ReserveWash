using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReserveWash.Models;
using ReserveWash.Repository.Services;
using ReserveWash.ViewModels.Product;

namespace ReserveWash
{
    public class CarController : Controller
    {
        private readonly CarService _carService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CarController(CarService carservice, UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager)
        {
            _carService = carservice;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: car
        public async Task<IActionResult> Index()
        {
            var carQuery = await _carService.GetAllAsync();
            var thisUser = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var caresDto = carQuery.Where(w => w.UserId == thisUser)
                .ToList()
                .Adapt<List<CarViewModel>>();

            return View(caresDto);
                          
        }

        // GET: car/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _carService.GetByIdAsync((int)id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var caresDto = content.UserId == currentUserId ? content.Adapt<CarViewModel>() : null;
            
            if (caresDto == null)
            {
                return NotFound();
            }

            return View(caresDto);
        }

        // GET: car/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: car/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( CarViewModel CarViewModel)
        {
            if (ModelState.IsValid)
            {
                string nowDate = DateTime.Now.ToString("yyyy", CultureInfo.InvariantCulture);
                var car = CarViewModel.Adapt<Car>();
                car.UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _carService.AddAsync(car);
                
                return RedirectToAction("Index");
            }

            return View(CarViewModel);
        }

        // GET: car/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var getedcar = await _carService.GetByIdAsync((int)id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var car = getedcar?.UserId == currentUserId ? getedcar : null;
            if (car == null)
            {
                return NotFound();
            }

            var carVM = car.Adapt<CarViewModel>();
            return View(carVM);
        }

        // POST: car/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarViewModel CarViewModel)
        {
            if (id != CarViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    CarViewModel.UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var car = CarViewModel.Adapt<Car>();

                    await _carService.UpdateAsync(car);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                        throw ex;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(CarViewModel);
        }

        // GET: car/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _carService.GetByIdAsync((int)id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            car = car.UserId == currentUserId ? car : null;
            if (car == null)
            {
                return NotFound();
            }

            var CarViewModel = car.Adapt<CarViewModel>();
            return View(CarViewModel);
        }

        // POST: car/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _carService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
