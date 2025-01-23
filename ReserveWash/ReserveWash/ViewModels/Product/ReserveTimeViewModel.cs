using System.ComponentModel.DataAnnotations;

namespace ReserveWash.ViewModels.Product
{
    public class ReserveTimeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام کارواش الزامی است.")]
        [Display(Name = "نام کارواش")]
        public string? CarwashName { get; set; }

        [Display(Name = "نام خدمت")]
        public string? ServiceName { get; set; }

        [Display(Name = "تاریخ نوبت")]
        public DateTime ReservationDate { get; set; }

        [Display(Name = "تاریخ نوبت")]
        public string ReservationDateFa { get; set; }

        [Display(Name = "زمان نوبت")]
        public string ReservationTime { get; set; }

        [Display(Name = "قابل رزرو است؟")]
        public bool IsReserved { get; set; }

        public int? ServiceId { get; set; }
        public int? CarwashId { get; set; }
    }
}
