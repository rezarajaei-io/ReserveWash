using Microsoft.AspNetCore.Mvc;
using ReserveWash.AI;
using ReserveWash.Repository.Services;

namespace ReserveWash
{
    // TODO : Implimentation
    public class MachineLearningController : Controller
    {
        private readonly MachineLearningService _machineLearningService;

        public MachineLearningController(MachineLearningService machineLearningService)
        {
            _machineLearningService = machineLearningService;
        }

        public IActionResult Predict()
        {
            // داده ورودی برای پیش‌بینی
            var inputData = new CustomerServiceData
            {
                CarId = 1,
                ServiceId = 2,
                ServiceDate = DateTime.Now.Ticks
            };

            // انجام پیش‌بینی
            var prediction = _machineLearningService.PredictServiceCost(inputData);

            // ارسال پیش‌بینی به ویو
            return View(prediction);
        }
    }


}
