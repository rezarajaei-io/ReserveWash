namespace ReserveWash.ViewModels.Product
{
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
        public int CarWashId { get; set; }
    }

}
