using System.ComponentModel.DataAnnotations;

namespace ReserveWash.Models
{
    public class Car
    {
        public int Id { get; set; }
        [MaxLength(15, ErrorMessage = "مدل ماشین نمیتواند بیشتر از 15 کاراکتر باشد")]
        public string CarModel { get; set; }
        [MaxLength(128, ErrorMessage = "برند نمیتواند بیشتر از 128 کاراکتر باشد")]
        public string Brand { get; set; }
        [MaxLength(128, ErrorMessage = "پلاک نمیتواند بیشتر از 128 کاراکتر باشد")]
        public string Pelak { get; set; } = "0";
        public string UserId { get; set; }

        public ApplicationUser User { get; set; } // ناوبری به کاربر

    }
}
