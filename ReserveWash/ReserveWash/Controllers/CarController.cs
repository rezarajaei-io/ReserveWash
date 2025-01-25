using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReserveWash.Models;
using ReserveWash.Repository.Services;
using ReserveWash.ViewModels.Product;

namespace ReserveWash
{
    public class CarController : Controller
    {
        private readonly CarService _carService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarController(CarService carservice, UserManager<ApplicationUser> userManager)
        {
            _carService = carservice;
            _userManager = userManager;
        }

        // Helper method to check if the user is an admin
        private async Task<bool> IsAdminAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        // GET: car
        public async Task<IActionResult> Index()
        {
            var carQuery = await _carService.GetAllAsync();
            var thisUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If user is admin, show all data
            if (await IsAdminAsync())
            {
                var caresDto = carQuery.ToList().Adapt<List<CarViewModel>>();
                return View(caresDto);
            }

            // For regular users, show only their own data
            var filteredCaresDto = carQuery
                .Where(w => w.UserId == thisUser)
                .ToList()
                .Adapt<List<CarViewModel>>();

            return View(filteredCaresDto);
        }

        // GET: car/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _carService.GetByIdAsync((int)id);

            // If user is admin, show the data
            if (await IsAdminAsync())
            {
                var caresDto = content.Adapt<CarViewModel>();
                return View(caresDto);
            }

            // For regular users, check ownership
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (content.UserId != currentUserId)
            {
                return Forbid();
            }

            var userCaresDto = content.Adapt<CarViewModel>();
            return View(userCaresDto);
        }

        // GET: car/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: car/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarViewModel carViewModel)
        {
            if (ModelState.IsValid)
            {
                var car = carViewModel.Adapt<Car>();
                car.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _carService.AddAsync(car);

                return RedirectToAction("Index");
            }

            return View(carViewModel);
        }

        // GET: car/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _carService.GetByIdAsync((int)id);

            // If user is admin, allow editing
            if (await IsAdminAsync())
            {
                var carVM = car.Adapt<CarViewModel>();
                return View(carVM);
            }

            // For regular users, check ownership
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (car.UserId != currentUserId)
            {
                return Forbid();
            }

            var userCarVM = car.Adapt<CarViewModel>();
            return View(userCarVM);
        }

        // POST: car/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarViewModel carViewModel)
        {
            if (id != carViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var car = await _carService.GetByIdAsync(id);

                // If user is admin, allow editing
                if (await IsAdminAsync())
                {
                    await _carService.UpdateAsync(carViewModel.Adapt<Car>());
                    return RedirectToAction(nameof(Index));
                }

                // For regular users, check ownership
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (car.UserId != currentUserId)
                {
                    return Forbid();
                }

                carViewModel.UserId = currentUserId;
                await _carService.UpdateAsync(carViewModel.Adapt<Car>());
                return RedirectToAction(nameof(Index));
            }

            return View(carViewModel);
        }

        // GET: car/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _carService.GetByIdAsync((int)id);

            // If user is admin, allow deletion
            if (await IsAdminAsync())
            {
                var carVM = car.Adapt<CarViewModel>();
                return View(carVM);
            }

            // For regular users, check ownership
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (car.UserId != currentUserId)
            {
                return Forbid();
            }

            var userCarVM = car.Adapt<CarViewModel>();
            return View(userCarVM);
        }

        // POST: car/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _carService.GetByIdAsync(id);

            // If user is admin, allow deletion
            if (await IsAdminAsync())
            {
                await _carService.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }

            // For regular users, check ownership
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (car.UserId != currentUserId)
            {
                return Forbid();
            }

            await _carService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
