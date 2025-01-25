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

        [Display(Name = "نام خودرو")]
        public string CarName { get; set; }
        [Display(Name = "شناسه کارواش")]
        public int? CarwashId { get; set; }
        [Display(Name = "پلاک خودرو")]
        public string CarPelak { get; set; }
        [Display(Name = "نام کارواش")]
        public string CarwashName { get; set; }

        [Display(Name = "نوع خدمت")]
        public string ServiceName { get; set; }

        [Display(Name = "زمان رزرو شده")]
        public string ReserveDateFa { get; set; }
        // Reservation Relation In VM
        public int? ReserveTimeId { get; set; }

        // متد برای تعریف تنظیمات مپینگ
        
    }
}
