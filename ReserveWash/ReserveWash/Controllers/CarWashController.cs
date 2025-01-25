using System.Globalization;
using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReserveWash.Models;
using ReserveWash.Repository.Services;
using ReserveWash.ViewModels.Product;

namespace ReserveWash
{
    public class CarWashController : Controller
    {
        private readonly CarwashService _carwashService;
        private readonly ReserveTimeService _reserveTimeService;
        private readonly ServiceRepository _serviceRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public CarWashController(
            CarwashService carwashService,
            ReserveTimeService reserveTimeService,
            ServiceRepository serviceRepository,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _carwashService = carwashService;
            _reserveTimeService = reserveTimeService;
            _serviceRepository = serviceRepository;
            _userManager = userManager;
            _environment = environment;
        }

        // Helper method to check if the user is an admin
        private async Task<bool> IsAdminAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        // GET: CarWash
        public async Task<IActionResult> Index()
        {
            var carwashQuery = await _carwashService.GetAllAsync();
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If user is admin, show all car washes
            if (await IsAdminAsync())
            {
                var carwashesDto = carwashQuery.Adapt<List<CarWashViewModel>>();
                return View(carwashesDto);
            }

            // For regular users, filter by UserId
            var userCarwashesDto = carwashQuery
                .Where(w => w.UserId == currentUserId)
                .ToList()
                .Adapt<List<CarWashViewModel>>();

            return View(userCarwashesDto);
        }

        // GET: Carwash/Create
        // GET: CarWash/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CarWash/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Name,Address,Description")] CarWashViewModel carWashViewModel,
            IFormFile? MainImage,
            IFormFile? SubImage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // تنظیم تاریخ
                    string nowDate = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    carWashViewModel.CreateDate = nowDate;

                    // آپلود عکس اصلی
                    if (MainImage != null && MainImage.Length > 0)
                    {
                        string uploadFolder = Path.Combine(_environment.WebRootPath, "Uploads", "MainImages");
                        Directory.CreateDirectory(uploadFolder);

                        string uniqueMainImageName = Guid.NewGuid().ToString() + "_" + MainImage.FileName;
                        string mainImagePath = Path.Combine(uploadFolder, uniqueMainImageName);

                        using (var fileStream = new FileStream(mainImagePath, FileMode.Create))
                        {
                            await MainImage.CopyToAsync(fileStream);
                        }

                        carWashViewModel.MainImagePath = Path.GetRelativePath(
                            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                            mainImagePath
                        ).Replace("\\", "/");
                    }
                    else
                    {
                        carWashViewModel.MainImagePath = null; // یا یک مسیر پیش‌فرض برای زمانی که فایلی ارسال نمی‌شود
                    }

                    // آپلود عکس فرعی
                    if (SubImage != null && SubImage.Length > 0)
                    {
                        string uploadFolder = Path.Combine(_environment.WebRootPath, "Uploads", "SubImages");
                        Directory.CreateDirectory(uploadFolder);

                        string uniqueSubImageName = Guid.NewGuid().ToString() + "_" + SubImage.FileName;
                        string subImagePath = Path.Combine(uploadFolder, uniqueSubImageName);

                        using (var fileStream = new FileStream(subImagePath, FileMode.Create))
                        {
                            await SubImage.CopyToAsync(fileStream);
                        }

                        carWashViewModel.SubImagePath = Path.GetRelativePath(
                            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                            subImagePath
                        ).Replace("\\", "/");
                    }
                    else
                    {
                        carWashViewModel.SubImagePath = null; // یا یک مسیر پیش‌فرض
                    }

                    // تبدیل ViewModel به Model
                    var carwashModel = carWashViewModel.Adapt<Carwash>();
                    carwashModel.UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    // ذخیره در پایگاه داده
                    await _carwashService.AddAsync(carwashModel);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // مدیریت خطا
                    ModelState.AddModelError(string.Empty, "خطایی در ایجاد کارواش رخ داده است: " + ex.Message);
                }
            }

            return View(carWashViewModel);
        }



        // GET: CarWash/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carwash = await _carwashService.GetByIdAsync((int)id);

            // If user is admin, allow access
            if (await IsAdminAsync())
            {
                var carwashDto = carwash.Adapt<CarWashViewModel>();
                return View(carwashDto);
            }

            // For regular users, check ownership
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (carwash.UserId != currentUserId)
            {
                return Forbid();
            }

            var userCarwashDto = carwash.Adapt<CarWashViewModel>();
            return View(userCarwashDto);
        }

        // GET: CarWash/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carwash = await _carwashService.GetByIdAsync((int)id);

            // If user is admin, allow editing
            if (await IsAdminAsync())
            {
                var carwashDto = carwash.Adapt<CarWashViewModel>();
                return View(carwashDto);
            }

            // For regular users, check ownership
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (carwash.UserId != currentUserId)
            {
                return Forbid();
            }

            var userCarwashDto = carwash.Adapt<CarWashViewModel>();
            return View(userCarwashDto);
        }

        // POST: CarWash/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarWashViewModel carWashViewModel, IFormFile? MainImage, IFormFile? SubImage)
        {
            if (id != carWashViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var carwash = await _carwashService.GetByIdAsync(id);
                if (carwash == null)
                {
                    return NotFound();
                }

                // If user is admin, allow editing
                if (await IsAdminAsync())
                {
                    await UpdateCarWashFiles(carWashViewModel, carwash, MainImage, SubImage);
                    return RedirectToAction(nameof(Index));
                }

                // For regular users, check ownership
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (carwash.UserId != currentUserId)
                {
                    return Forbid();
                }

                carWashViewModel.UserId = currentUserId;
                await UpdateCarWashFiles(carWashViewModel, carwash, MainImage, SubImage);
                return RedirectToAction(nameof(Index));
            }

            return View(carWashViewModel);
        }

        private async Task UpdateCarWashFiles(CarWashViewModel carWashViewModel, Carwash carwash, IFormFile? MainImage, IFormFile? SubImage)
        {
            // Update Main Image
            if (MainImage != null && MainImage.Length > 0)
            {
                string uploadFolder = Path.Combine(_environment.WebRootPath, "Uploads", "MainImages");
                Directory.CreateDirectory(uploadFolder);

                string uniqueMainImageName = Guid.NewGuid().ToString() + "_" + MainImage.FileName;
                string mainImagePath = Path.Combine(uploadFolder, uniqueMainImageName);

                using (var fileStream = new FileStream(mainImagePath, FileMode.Create))
                {
                    await MainImage.CopyToAsync(fileStream);
                }

                carwash.MainImagePath = Path.GetRelativePath(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                    mainImagePath
                ).Replace("\\", "/");
            }

            // Update Sub Image
            if (SubImage != null && SubImage.Length > 0)
            {
                string uploadFolder = Path.Combine(_environment.WebRootPath, "Uploads", "SubImages");
                Directory.CreateDirectory(uploadFolder);

                string uniqueSubImageName = Guid.NewGuid().ToString() + "_" + SubImage.FileName;
                string subImagePath = Path.Combine(uploadFolder, uniqueSubImageName);

                using (var fileStream = new FileStream(subImagePath, FileMode.Create))
                {
                    await SubImage.CopyToAsync(fileStream);
                }

                carwash.SubImagePath = Path.GetRelativePath(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                    subImagePath
                ).Replace("\\", "/");
            }

            // Update other properties from ViewModel
            carwash.Name = carWashViewModel.Name;
            carwash.Address = carWashViewModel.Address;
            carwash.Description = carWashViewModel.Description;

            await _carwashService.UpdateAsync(carwash);
        }


        // GET: CarWash/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carwash = await _carwashService.GetByIdAsync((int)id);

            // If user is admin, allow deletion
            if (await IsAdminAsync())
            {
                var carwashDto = carwash.Adapt<CarWashViewModel>();
                return View(carwashDto);
            }

            // For regular users, check ownership
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (carwash.UserId != currentUserId)
            {
                return Forbid();
            }

            var userCarwashDto = carwash.Adapt<CarWashViewModel>();
            return View(userCarwashDto);
        }

        // POST: CarWash/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var content = await _carwashService.GetAllAsync();
            content = content.Where(w => w.Id == id).Include(i => i.ReserveTime).Include(i => i.Services);
            var reserveTimes = content?.SelectMany(m => m.ReserveTime).AsEnumerable();
            var services = content?.SelectMany(m => m.Services).AsEnumerable();

            if (reserveTimes.Any())
            {
                await _reserveTimeService.DeleteAsync(reserveTimes);
            }

            if (services.Any())
            {
                await _serviceRepository.DeleteAsync(services);
            }
            await _carwashService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
