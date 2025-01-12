namespace ReserveWash.ViewModels.Product
{
    using System.ComponentModel.DataAnnotations;

    public class FeedbackViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نظرات الزامی است.")]
        [Display(Name = "نظرات")]
        public string Comments { get; set; }
        // شناسه کارواش مربوطه
        public int CarWashId { get; set; }
        // نام کاربری یا نام کامل کاربر (اختیاری)
        [Display(Name = "نام کاربر")]
        public string UserName { get; set; }
    }

}
