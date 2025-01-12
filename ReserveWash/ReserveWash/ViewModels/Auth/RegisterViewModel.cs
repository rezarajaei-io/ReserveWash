using System.ComponentModel.DataAnnotations;

namespace ReserveWash.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "ایمیل الزامی است.")]
        [EmailAddress(ErrorMessage = "ایمیل معتبر نیست.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "نام کامل الزامی است.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "رمز عبور الزامی است.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تأیید رمز عبور")]
        [Compare("Password", ErrorMessage = "رمز عبور و تأیید آن مطابقت ندارند.")]
        public string ConfirmPassword { get; set; }
    }

}
