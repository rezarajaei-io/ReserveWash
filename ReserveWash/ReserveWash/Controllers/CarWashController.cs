using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using ReserveWash.Models;
using ReserveWash.Repository.Services;
using ReserveWash.ViewModels.Product;

namespace ReserveWash
{
    public class CarWashController : Controller
    {
        private readonly CarwashService _carwashService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _environment;

        public CarWashController(CarwashService carwashservice, UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment environment)
        {
            _carwashService = carwashservice;
            _signInManager = signInManager;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: CarWash
        public async Task<IActionResult> Index()
        {
            var carwashQuery = await _carwashService.GetAllAsync();
            var thisUser = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var carwashesDto = carwashQuery.Where(w => w.UserId == thisUser)
                .ToList()
                .Adapt<List<CarWashViewModel>>();

            return View(carwashesDto);

        }

        // GET: CarWash/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var content = await _carwashService.GetByIdAsync((int)id);
            var carwashesDto = content.Adapt<CarWashViewModel>();

            if (carwashesDto == null)
            {
                return NotFound();
            }

            return View(carwashesDto);
        }

        // GET: CarWash/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CarWash/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
        [Bind("Id,Name,Address,Star,MainImage,SubImage")] CarWashViewModel carWashViewModel,
        IFormFile MainImage,
        IFormFile SubImage)
        {
            if (ModelState.IsValid)
            {
                // تنظیم تاریخ
                string nowDate = DateTime.Now.ToString("yyyy-mm", CultureInfo.InvariantCulture);
                carWashViewModel.CreateDate = nowDate;

                // آپلود لوگو
                if (MainImage != null && MainImage.Length > 0)
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "Uploads", "MainImages");
                    Directory.CreateDirectory(uploadFolder);

                    string uniqueMainImageName = Guid.NewGuid().ToString() + "_" + MainImage.FileName;
                    string MainImagePath = Path.Combine(uploadFolder, uniqueMainImageName);

                    using (var fileStream = new FileStream(MainImagePath, FileMode.Create))
                    {
                        await MainImage.CopyToAsync(fileStream);
                    }
                    carWashViewModel.MainImagePath = Path.GetRelativePath(
                        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                        MainImagePath
                    ).Replace("\\", "/");

                }

                // آپلود عکس فرعی
                if (SubImage != null && SubImage.Length > 0)
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "Uploads", "SubImages");
                    Directory.CreateDirectory(uploadFolder);

                    string uniqueSubImageName = Guid.NewGuid().ToString() + "_" + SubImage.FileName;
                    string SubImagePath = Path.Combine(uploadFolder, uniqueSubImageName);

                    using (var fileStream = new FileStream(SubImagePath, FileMode.Create))
                    {
                        await SubImage.CopyToAsync(fileStream);
                    }
                    carWashViewModel.SubImagePath = Path.GetRelativePath(
                      Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                      SubImagePath
                  ).Replace("\\", "/");
                }

                // تبدیل ViewModel به Model
                var carwashModel = carWashViewModel.Adapt<Carwash>();
                carwashModel.UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

                await _carwashService.AddAsync(carwashModel);

                return RedirectToAction("Index");
            }

            return View(carWashViewModel);
        }
        // GET: CarWash/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carwash = await _carwashService.GetByIdAsync((int)id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            carwash = carwash.UserId == currentUserId ? carwash : null;

            if (carwash == null)
            {
                return NotFound();
            }

            var carwashVM = carwash.Adapt<CarWashViewModel>();

            // اضافه کردن مسیرهای عکس به ViewModel
            carwashVM.MainImagePath = carwash.MainImagePath;
            carwashVM.SubImagePath = carwash.SubImagePath;

            return View(carwashVM);
        }

        // POST: CarWash/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Name,Address,Star,CreateDate")] CarWashViewModel carWashViewModel,
            IFormFile MainImage,
            IFormFile SubImage)
        {
            if (id != carWashViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // بازیابی مدل موجود
                    var existingCarwash = await _carwashService.GetByIdAsync(id);

                    // آپلود لوگو
                    if (MainImage != null && MainImage.Length > 0)
                    {
                        string uploadFolder = Path.Combine(_environment.WebRootPath, "Uploads", "MainImages");
                        Directory.CreateDirectory(uploadFolder);

                        string uniqueMainImageName = Guid.NewGuid().ToString() + "_" + MainImage.FileName;
                        string MainImagePath = Path.Combine(uploadFolder, uniqueMainImageName);

                        using (var fileStream = new FileStream(MainImagePath, FileMode.Create))
                        {
                            await MainImage.CopyToAsync(fileStream);
                        }
                        carWashViewModel.MainImagePath = Path.Combine("Uploads", "MainImages", Path.GetFileName(MainImagePath)).Replace("\\", "/");
                    }
                    else
                    {
                        // حفظ تصویر قبلی
                        carWashViewModel.MainImagePath = existingCarwash.MainImagePath;
                    }

                    // آپلود عکس فرعی
                    if (SubImage != null && SubImage.Length > 0)
                    {
                        string uploadFolder = Path.Combine(_environment.WebRootPath, "Uploads", "SubImages");
                        Directory.CreateDirectory(uploadFolder);

                        string uniqueSubImageName = Guid.NewGuid().ToString() + "_" + SubImage.FileName;
                        string SubImagePath = Path.Combine(uploadFolder, uniqueSubImageName);

                        using (var fileStream = new FileStream(SubImagePath, FileMode.Create))
                        {
                            await SubImage.CopyToAsync(fileStream);
                        }
                        carWashViewModel.SubImagePath = Path.Combine("Uploads", "SubImages", Path.GetFileName(SubImagePath)).Replace("\\", "/");
                    }
                    else
                    {
                        // حفظ تصویر قبلی
                        carWashViewModel.SubImagePath = existingCarwash.SubImagePath;
                    }

                    var carwash = carWashViewModel.Adapt<Carwash>();
                    carwash.UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    await _carwashService.UpdateAsync(carwash);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    throw ex;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(carWashViewModel);
        }


        //// GET: CarWash/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var carwash = await _carwashService.GetByIdAsync((int)id);
        //    var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    carwash = carwash.UserId == currentUserId ? carwash : null;
        //    if (carwash == null)
        //    {
        //        return NotFound();
        //    }

        //    var carwashVM = carwash.Adapt<CarWashViewModel>();
        //    return View(carwashVM);
        //}

        //// POST: CarWash/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Star,CreateDate")] CarWashViewModel carWashViewModel)
        //{
        //    if (id != carWashViewModel.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var carwash = carWashViewModel.Adapt<Carwash>();
        //            carwash.UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //            await _carwashService.UpdateAsync(carwash);
        //        }
        //        catch (DbUpdateConcurrencyException ex)
        //        {
        //            throw ex;
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(carWashViewModel);
        //}

        // GET: CarWash/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carwash = await _carwashService.GetByIdAsync((int)id);
            var currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            carwash = carwash.UserId == currentUserId ? carwash : null;
            if (carwash == null)
            {
                return NotFound();
            }

            var carWashViewModel = carwash.Adapt<CarWashViewModel>();
            return View(carWashViewModel);
        }

        // POST: CarWash/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _carwashService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
