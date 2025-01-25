using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReserveWash.Repository.Services;
using ReserveWash.Utilities;
using ReserveWash.ViewModels.Product;

namespace ReserveWash.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ReservationService _reservationService;

        public AdminDashboardController(ReservationService reservationRepository)
        {
            _reservationService = reservationRepository;
        }

        public async Task<IActionResult> Index()
        {
            var reservations = await _reservationService.GetAllAsync();

            reservations = reservations.Include(i => i.ReserveTime)
                                       .ThenInclude(t => t.Service)
                                       .Include(i => i.Car);

            var reservationList = await reservations.ToListAsync(); // دریافت تمام داده‌ها از دیتابیس

            var persianMonthsOrder = new List<string>
{
    "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
    "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
};

            var viewModel = new AdminDashboardViewModel
            {
                TotalReservations = reservationList.Count,
                TotalIncome = reservationList
                    .Where(r => r.ReserveTime != null && r.ReserveTime.Service != null)
                    .Sum(r => r.ReserveTime.Service.Price),
                CarsCount = reservationList
                    .Where(r => r.Car != null)
                    .Select(r => r.Car)
                    .Distinct()
                    .Count(),
                RecentReservations = reservationList
                    .Where(r => r.ReserveTime != null && r.ReserveTime.Service != null && r.Car != null)
                    .OrderByDescending(r => r.ReserveTime.ReservationDate)
                    .Take(5)
                    .Select(r => new ReservationInfo
                    {
                        CarName = r.Car.Brand,
                        ServiceName = r.ReserveTime.Service.Name,
                        ReservationDate = r.ReserveTime.ReservationDate
                    }).ToList(),
                PopularServices = reservationList
                    .Where(r => r.ReserveTime != null && r.ReserveTime.Service != null)
                    .GroupBy(r => r.ReserveTime.Service.Name)
                    .Select(g => new ServicePopularity
                    {
                        ServiceName = g.Key,
                        UsageCount = g.Count()
                    }).OrderByDescending(s => s.UsageCount).ToList(),
                MonthlyIncome = reservationList
                    .Where(r => r.ReserveTime != null && r.ReserveTime.Service != null)
                    .GroupBy(r => DateConverter.GregorianToJalaliStringPersianMonth(r.ReserveTime.ReservationDate)) // تبدیل به نام ماه شمسی
                    .Select(g => new IncomeData
                    {
                        Month = g.Key,
                        Income = g.Sum(r => r.ReserveTime.Service.Price)
                    })
                    .OrderBy(m => persianMonthsOrder.IndexOf(m.Month)) // مرتب‌سازی بر اساس ماه‌های شمسی
                    .ToList()
            };


            return View(viewModel);
        }
    }

}
