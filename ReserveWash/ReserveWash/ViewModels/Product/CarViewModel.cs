using System.ComponentModel.DataAnnotations;

namespace ReserveWash.ViewModels.Product
{

    public class CarViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "مدل خودرو الزامی است.")]
        [Display(Name = "مدل خودرو")]
        public string CarModel { get; set; }

        [Required(ErrorMessage = "نام خودرو الزامی است.")]
        [Display(Name = "نام خودرو")]
        public string Brand { get; set; }
        [Display(Name = "پلاک")]
        public string Pelak { get; set; }
        public string? UserId { get; set; }
    }

}
