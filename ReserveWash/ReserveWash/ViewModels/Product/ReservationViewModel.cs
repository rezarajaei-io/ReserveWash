namespace ReserveWash.ViewModels.Product
{
    using Mapster;
    using ReserveWash.Models;
    using ReserveWash.Utilities;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ReservationViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "تاریخ رزرو الزامی است.")]
        [Display(Name = "تاریخ رزرو")]
        public DateTime ReservationDate { get; set; }

        // شناسه خودرو مربوطه
        [Required(ErrorMessage = "شناسه خودرو الزامی است.")]
        [Display(Name = "شناسه خودرو")]
        public int CarId { get; set; }

        // شناسه خدمت مربوطه
        [Required(ErrorMessage = "شناسه خدمت الزامی است.")]
        [Display(Name = "شناسه خدمت")]
        public int ServiceId { get; set; }

        // شناسه کارواش مربوطه (اختیاری)
        public int CarwashId { get; set; }

        [Display(Name = "نام کارواش")]
        public string CarwashName { get; set; }

        [Display(Name = "نوع خدمت")]
        public string ServiceName { get; set; }

        [Display(Name = "زمان رزرو شده")]
        public string ReserveDateFa { get; set; }

        // متد برای تعریف تنظیمات مپینگ
        public static void ConfigureMapping()
        {
            TypeAdapterConfig<Reservation, ReservationViewModel>
                .NewConfig()
                .Map(dest => dest.CarwashName, src => src.Carwash.Name) // مپ کردن نام کارواش
                .Map(dest => dest.ServiceName, src => src.Service.Name) // مپ کردن نام خدمت
                .Map(dest => dest.ReserveDateFa, src => DateConverter.GregorianToJalaliStringWithTime(src.ReservationDate)); // تبدیل تاریخ به شمسی
        }
    }
}
